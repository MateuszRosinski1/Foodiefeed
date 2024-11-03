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
        public async Task<IActionResult> GetCommentById([FromRoute]int commentId)
        {
            var response  = await commentService.GetCommentById(commentId);
            return Ok(response);
        }

        [HttpPost("add-new-comment-{postId}")]
        public async Task<IActionResult> AddNewComment([FromBody] NewCommentDto dto, [FromRoute] int postId)
        {
            await commentService.AddNewComment(postId, dto);
            return NoContent();
        }
    }
}
