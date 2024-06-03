using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("/api/posts")]
    public class PostController : ControllerBase
    {

        private readonly IPostService _postService;
        public PostController(IPostService service)
        {
            _postService = service;
        }

        [HttpGet("{userId}")]
        public IActionResult getAllPostForSpecificUser([FromRoute]int userId)
        {
            var results = _postService.getAllPostForUser(userId);
            return Ok(results);
        }

        [HttpGet("/wall/{userId}")]
        public void wallInit([FromRoute] int userId)
        {
            var results = _postService.wallInit(userId);
        }

        [HttpGet("/scrollFill/{userId}")]
        public void wallFill([FromRoute] int userId)
        {
            var results = _postService.wallFill(userId);
        }


    }
}
