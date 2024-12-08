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
    public async Task<IActionResult> SaveRecipe([FromRoute] int userId, [FromRoute] int postId)
    {
        await recipeService.SaveRecipe(userId,postId);
        return NoContent();
    }

    [HttpDelete("delete-saved/{postId}/{userId}")]
    public async Task<IActionResult> RemoveRecipe([FromRoute] int postId, [FromRoute]int userId)
    {
        await recipeService.RemoveRecipe(postId,userId);
        return NoContent();
    }

    [HttpGet("get-liked/{userId}/{lastRecipeId}")]
    public async Task<IActionResult> GetLikedRecipes([FromRoute] int userId, [FromRoute] int lastRecipeId, CancellationToken token)
    {
        var response  =  await recipeService.GetLikedRecipes(userId,lastRecipeId, token);
        return Ok(response);
    }

    [HttpGet("get-saved/{userId}/{lastRecipeId}")]
    public async Task<IActionResult> GetSavedRecipes([FromRoute] int userId,[FromRoute]int lastRecipeId, CancellationToken token)
    {
        var response = await recipeService.GetSavedRecipes(userId,lastRecipeId, token);
        return Ok(response);
    }
}
