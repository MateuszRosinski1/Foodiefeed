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

        [HttpGet("get-all-for-user/{userId}")]
        public async Task<IActionResult> GetNotoficationForuserById([FromRoute]int userID)
        {
            var response = await _notificationService.GetNotificationByUserId(userID);

            return Ok(response);
        }
    }
}
