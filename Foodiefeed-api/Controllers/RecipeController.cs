using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService recipeService;
    
    public RecipeController(IRecipeService _recipeService)
    {
        recipeService = _recipeService;
    }

    [HttpPost("save/{userId}/{postId}")]
    public async Task<IActionResult> SaveRecipe(int userId,int postId)
    {
        await recipeService.SaveRecipe(userId,postId);
        return NoContent();
    }

    [HttpDelete("delete/{recipeId}")]
    public async Task<IActionResult> RemoveRecipe(int recipeId)
    {
        await recipeService.RemoveRecipe(recipeId);
        return NoContent();
    }
}
