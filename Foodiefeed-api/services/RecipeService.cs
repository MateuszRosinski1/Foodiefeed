using Foodiefeed_api.entities;
using Foodiefeed_api.exceptions;

namespace Foodiefeed_api.services;

public interface IRecipeService
{
    public Task SaveRecipe(int userId,int postId);
    public Task RemoveRecipe(int recipeId);
}

public class RecipeService : IRecipeService
{
    private readonly dbContext dbContext;

    public RecipeService(dbContext _dbContext)
    {
        dbContext = _dbContext;
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
}
