using Microsoft.AspNetCore.Mvc;
using Foodiefeed_api.services;

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
        public IActionResult GetOnlineFriends(int userId)
        {
            var onlineFriends = _friendService.GetOnlineFriends(userId);

            return Ok(onlineFriends);
        }
        [HttpGet("offline/{userId}")]
        public IActionResult GetOfflineFriends(int userId)
        {
            return Ok();
        }
    }
}
