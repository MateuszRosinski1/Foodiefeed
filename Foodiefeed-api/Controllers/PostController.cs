using Foodiefeed_api.models.posts;
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
        public async Task<IActionResult> GetProfilePosts([FromRoute]int userId,int pageNumber)
        {
            var response = await _postService.GetProfilePostsAsync(userId,pageNumber);

            return Ok(response);
        }

        [HttpGet("popup-post/{postId}/{commentId}")]
        public async Task<IActionResult> GetPopupPost([FromRoute]int postId, [FromRoute]int commentId)
        {
            var response = await _postService.GetPopupPostAsync(postId, commentId);

            return Ok(response);
        }

        [HttpGet("popup-liked-post/{postId}")]
        public async Task<IActionResult> GetLikedPost([FromRoute]int postId)
        {
            var response  = await _postService.GetLikedPostAsync(postId);

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm]CreatePostDto dto)
        {
            await _postService.CreatePostAsync(dto);
            return NoContent();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePost(int postId,int userId)
        {
            await _postService.DeletePostAsync(postId,userId);
            return NoContent();
        }

        [HttpPost("generate-wall-posts")]
        public async Task<IActionResult> GenerateWallPosts(int userId,[FromBody]List<int> viewedPostsId)
        {
            var response = await _postService.GenerateWallPostsAsync(userId,viewedPostsId);
            return Ok(response);
        }

        //[HttpDelete("delete-like/{postId}/{userId}")]
        //public async Task<IActionResult> DeletePostLike([FromRoute]int postId,[FromRoute]int userId)
        //{
        //    await _postService.DeletePostLikeAsync(postId, userId);
        //    return NoContent();
        //}

        [HttpPost("like-post/{userId}")]
        public async Task<IActionResult> LikePost([FromRoute] int userId,int postId)
        {
            await _postService.LikePost(userId, postId);
            return NoContent();
        }

        [HttpDelete("unlike-post/{userId}")]
        public async Task<IActionResult> UnlikePost([FromRoute] int userId, int postId)
        {
            await _postService.UnlikePost(userId, postId);
            return NoContent();
        }
    }
    
}
