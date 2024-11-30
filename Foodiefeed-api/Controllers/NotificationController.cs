using Foodiefeed_api.services;
using Microsoft.AspNetCore.Mvc;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("get-15/{userId}")]
        public async Task<IActionResult> GetNotoficationForuserById([FromRoute]int userID,int pageNumber)
        {
            var response = await _notificationService.GetNotificationByUserId(userID,pageNumber);

            return Ok(response);
        }

        [HttpDelete("remove-range-notifications/{userId}")]
        public async Task<IActionResult> NotificationsRemoveRange([FromRoute]int userId,[FromBody]List<int> notificationsId)
        {
            await _notificationService.RemoveRange(notificationsId);
            return Ok();
        }
    }
}
