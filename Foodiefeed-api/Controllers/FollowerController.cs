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
        public async Task<IActionResult> GetUserFollowers([FromRoute]int id, CancellationToken token)
        {
            var response = await _followerService.GetFollowerListAsync(Convert.ToInt32(id),token);

            return Ok(response);
        }

        [HttpPost("follow/{userId}/{followedUserId}")]
        public async Task<IActionResult> FollowUser([FromRoute]int userId, [FromRoute]int followedUserId)
        {
            await _followerService.Follow(userId,followedUserId);
            return NoContent();
        }   

        [HttpDelete("unfollow/{userId}/{unfollowedUserId}")]
        public async Task<IActionResult> UnfollowUser([FromRoute] int userId, [FromRoute] int unfollowedUserId)
        {
            await _followerService.Unfollow(userId,unfollowedUserId);

            return NoContent();
        }
    }
}
