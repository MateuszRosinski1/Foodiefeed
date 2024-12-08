using Foodiefeed_api.models.comment;
using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentController : ControllerBase
    {

        private readonly ICommentService commentService;

        public CommentController(ICommentService _commentService)
        {
            commentService = _commentService;
        }

        [HttpGet("get-comment-{commentId}")]
        public async Task<IActionResult> GetCommentById([FromRoute]int commentId, CancellationToken token)
        {
            var response  = await commentService.GetCommentById(commentId, token);
            return Ok(response);
        }

        [HttpPost("add-new-comment-{postId}")]
        public async Task<IActionResult> AddNewComment([FromBody] NewCommentDto dto, [FromRoute] int postId)
        {
            var response = await commentService.AddNewComment(postId, dto);
            return Ok(response);
        }

        [HttpPut("edit-comment-{commentId}")]
        public async Task<IActionResult> EditComment([FromRoute]int commentId,[FromBody]string newContent)
        {
            await commentService.EditComment(commentId, newContent);
            return NoContent();
        }

        [HttpDelete("delete-comment-{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute]int commentId)
        {
            await commentService.DeleteComment(commentId);
            return NoContent();
        }

        [HttpPost("like-comment/{userId}")]
        public async Task<IActionResult> LikeComment([FromRoute]int userId,int commentId)
        {
            var postId = await commentService.LikeComment(userId, commentId);
            return Ok(postId);
        }

        [HttpDelete("unlike-comment/{userId}")]
        public async Task<IActionResult> UnlikeComment([FromRoute] int userId, int commentId)
        {
            var postId = await commentService.UnlikeComment(userId, commentId);
            return Ok(postId);
        }
    }
}
