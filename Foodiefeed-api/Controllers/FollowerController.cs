using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/followers")]
    public class FollowerController : ControllerBase
    {
        private readonly IFollowerService _followerService;

        public FollowerController(IFollowerService followerService)
        {
            _followerService = followerService;
        }


        [HttpGet("get-user-followers/{id}")]
        public async Task<IActionResult> GetUserFollowers([FromRoute]int id)
        {
            var response = await _followerService.GetFollowerListAsync(Convert.ToInt32(id));

            return Ok(response);
        }
    }
}
