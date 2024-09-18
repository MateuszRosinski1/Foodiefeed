using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("profile-posts/{userId}")]
        public async Task<IActionResult> GetProfilePosts([FromRoute]string userId)
        {
            var response = await _postService.GetProfilePostsAsync(userId);

            return Ok(response);
        }
    }
}
