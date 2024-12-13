using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.recipe;
using Microsoft.EntityFrameworkCore;

namespace Foodiefeed_api.services;

public interface IRecipeService
{
    public Task SaveRecipe(int userId,int postId);
    public Task RemoveRecipe(int postId, int userId);

    public Task<List<RecipeDto>> GetLikedRecipes(int userId, int lastId, CancellationToken token);
    public Task<List<RecipeDto>> GetSavedRecipes(int userId, int lastId, CancellationToken token);
}

public class RecipeService : IRecipeService
{
    private readonly dbContext dbContext;
    private readonly IMapper mapper;
    private readonly IAzureBlobStorageSerivce AzureBloBStorageService;

    public RecipeService(dbContext _dbContext,IMapper _mapper,IAzureBlobStorageSerivce _IAzureBlobStorageSerivce)
    {
        dbContext = _dbContext;
        mapper = _mapper;
        AzureBloBStorageService = _IAzureBlobStorageSerivce;
    }

    public async Task RemoveRecipe(int postId,int userId)
    {
        var recipe = dbContext.Recipes.FirstOrDefault(r => r.PostId == postId && r.UserId == userId);

        if (recipe is null) throw new BadRequestException("Recipe do not exist in current context");

        dbContext.Recipes.Remove(recipe);
        dbContext.SaveChanges();
    }

    public async Task SaveRecipe(int userId, int postId)
    {
        var recipe = new Recipe() { UserId = userId, PostId = postId };

        var existingRecipe = dbContext.Recipes.FirstOrDefault(r => r.UserId ==  userId && r.PostId == postId);

        if (existingRecipe is not null) throw new BadRequestException("recipe already exist");

        dbContext.Recipes.Add(recipe);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<RecipeDto>> GetLikedRecipes(int userId,int lastId, CancellationToken token)
    {
        var likedpost = dbContext.PostLikes.Where(p => p.UserId == userId).ToList();

        var postsIds = likedpost.Select(p => p.PostId).ToList();

        return await ExtractRecipes(postsIds,lastId, token);
    }

    public async Task<List<RecipeDto>> GetSavedRecipes(int userId,int lastId, CancellationToken token)
    {
        var savedrecipes = dbContext.Recipes.Where(r => r.UserId == userId);

        var postsIds = savedrecipes.Select(p => p.PostId).ToList();

        return await ExtractRecipes(postsIds,lastId, token);
    }

    /// <summary>
    /// Extracts 10 recipes based on the last recipe id (keyset pagination)
    /// </summary>
    /// <param name="postIds"></param>
    /// <returns>a List of with 10 or less elements</returns>
    private async Task<List<RecipeDto>> ExtractRecipes(List<int> postIds, int lastId, CancellationToken token)
    {
        var posts = dbContext.Posts
            .Include(p => p.PostProducts)
                .ThenInclude(pp => pp.Product)
            .Include(p => p.User)
            .OrderBy(p => p.PostId)
            .Where(p => postIds.Contains(p.PostId) && p.PostId > lastId).Take(10);

        token.ThrowIfCancellationRequested();

        var recipes = mapper.Map<List<RecipeDto>>(posts);

        foreach(var recipe in recipes)
        {
            recipe.ImageBase64 = await AzureBloBStorageService.FetchRecipeImageAsync(recipe.Id, recipe.UserId, token);
        }

        return recipes;
    }
}
