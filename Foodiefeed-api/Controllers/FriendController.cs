using Microsoft.AspNetCore.Mvc;
using Foodiefeed_api.services;
using Foodiefeed_api.models.friends;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/friends")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpGet("online/{userId}")]
        public async Task<ActionResult<List<ListedFriendDto>>> GetOnlineFriends(int userId)
        {
            var onlineFriends = await _friendService.GetOnlineFriends(userId);

            return Ok(onlineFriends);
        }

        [HttpGet("offline/{userId}")]
        public async Task<ActionResult<List<ListedFriendDto>>> GetOfflineFriends(int userId)
        {
            var offlineFriends = await _friendService.GetOfflineFriends(userId);
            return Ok(offlineFriends);
        }
    }
}
