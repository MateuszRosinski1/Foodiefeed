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

        [HttpGet("profile-friends/{userId}")]
        public async Task<IActionResult> GetProfileFriends([FromRoute]int userId, CancellationToken token)
        {
            var response = await _friendService.GetUserFriends(userId, token);

            return Ok(response);
        }

        [HttpGet("online/{userId}")]
        public async Task<ActionResult<List<ListedFriendDto>>> GetOnlineFriends([FromRoute] int userId, CancellationToken token)
        {
            var onlineFriends = await _friendService.GetOnlineFriends(userId, token);

            return Ok(onlineFriends);
        }

        [HttpGet("offline/{userId}")]
        public async Task<ActionResult<List<ListedFriendDto>>> GetOfflineFriends([FromRoute] int userId, CancellationToken token)
        {
            var offlineFriends = await _friendService.GetOfflineFriends(userId, token);
            return Ok(offlineFriends);
        }

        [HttpPost("add/{senderId}/{receiverId}")]
        public async Task<IActionResult> SendFriendRequest([FromRoute]int senderId, [FromRoute] int receiverId)
        {
            await _friendService.SendFriendRequest(senderId, receiverId);

            return Ok("Friend added succesfuly");
        }

        [HttpPost("request/accept/{senderId}/{receiverId}")]
        public async Task<IActionResult> AcceptFriendRequest([FromRoute] int senderId, [FromRoute] int receiverId)
        {
            await _friendService.AcceptFriendRequest(senderId, receiverId);

            return Ok($"User {receiverId} accepted friend request from {senderId}");
        }

        [HttpDelete("request/delete/{senderId}/{receiverId}")]
        public async Task<IActionResult> DeclineFriendRequest([FromRoute] int senderId, [FromRoute] int receiverId)
        {
            await _friendService.DeclineFriendRequest(senderId, receiverId);
            return Ok($"User {receiverId} declined friend reqest from {senderId}");
        }

        [HttpDelete("request/cancel/{senderId}/{receiverId}")]
        public async Task<IActionResult> CancelFriendRequest([FromRoute] int senderId, [FromRoute] int receiverId)
        {
            await _friendService.CancelFriendRequest(senderId, receiverId);

            return Ok("Request deleted succesfuly");
        }

        [HttpDelete("/unfriend{userId}/{friendId}")]
        public async Task<IActionResult> Unfriend([FromRoute]int userId, [FromRoute]int friendId)
        {
            await _friendService.Unfriend(userId, friendId);

            return NoContent();
        }
    }
}
