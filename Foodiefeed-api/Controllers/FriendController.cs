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
        public async Task<ActionResult<List<ListedFriendDto>>> GetOnlineFriends([FromRoute] int userId)
        {
            var onlineFriends = await _friendService.GetOnlineFriends(userId);

            return Ok(onlineFriends);
        }

        [HttpGet("offline/{userId}")]
        public async Task<ActionResult<List<ListedFriendDto>>> GetOfflineFriends([FromRoute] int userId)
        {
            var offlineFriends = await _friendService.GetOfflineFriends(userId);
            return Ok(offlineFriends);
        }

        [HttpPost("add/{senderId}/{receiverId}")]
        public async Task<IActionResult> SendFriendRequest([FromRoute]int senderId, [FromRoute] int receiverId)
        {
            await _friendService.SendFriendRequest(senderId, receiverId);

            return Ok("Friend added succesfuly");
        }

        [HttpPost("accept/{senderId}/{receiverId}")]
        public async Task<IActionResult> AcceptFriendRequest([FromRoute] int senderId, [FromRoute] int receiverId)
        {
            await _friendService.AcceptFriendRequest(senderId, receiverId);

            return Ok($"User {receiverId} accepted friend request from {senderId}");
        }
    }
}
