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
    }
}
