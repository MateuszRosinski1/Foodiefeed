using AutoMapper;
using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;
using Foodiefeed_api.models.recipe;
using Microsoft.EntityFrameworkCore;

namespace Foodiefeed_api.services;

public interface IRecipeService
{
    public Task SaveRecipe(int userId,int postId);
    public Task RemoveRecipe(int recipeId);

    public Task<List<RecipeDto>> GetLikedRecipes(int userId);
    public Task<List<RecipeDto>> GetSavedRecipes(int userId);
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

    public async Task RemoveRecipe(int recipeId)
    {
        var recipe = dbContext.Recipes.FirstOrDefault(r => r.Id == recipeId);

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

    public async Task<List<RecipeDto>> GetLikedRecipes(int userId)
    {
        var likedpost = dbContext.PostLikes.Where(p => p.UserId == userId).ToList();

        var postsIds = likedpost.Select(p => p.PostId).ToList();

        return await ExtractRecipes(postsIds);
    }

    public async Task<List<RecipeDto>> GetSavedRecipes(int userId)
    {
        var savedrecipes = dbContext.Recipes.Where(r => r.UserId == userId);

        var postsIds = savedrecipes.Select(p => p.PostId).ToList();

        return await ExtractRecipes(postsIds);
    }

    private async Task<List<RecipeDto>> ExtractRecipes(List<int> postIds)
    {
        var posts = dbContext.Posts
            .Include(p => p.PostProducts)
                .ThenInclude(pp => pp.Product)
            .Include(p => p.User)
            .Where(p => postIds.Contains(p.PostId));

        var recipes = mapper.Map<List<RecipeDto>>(posts);

        foreach(var recipe in recipes)
        {
            var stream = await AzureBloBStorageService.FetchRecipeImage(recipe.Id,recipe.UserId);
            recipe.ImageBase64 = await AzureBloBStorageService.ConvertStreamToBase64Async(stream);
        }

        return recipes;
    }
}
