using Microsoft.AspNetCore.Mvc;
using Foodiefeed_api.services;
using Foodiefeed_api.models.user;
using Microsoft.AspNetCore.Authorization;

namespace Foodiefeed_api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById([FromRoute]string userId) {
            var response = await _service.GetById(userId);

            return Ok(response);
        }

        [HttpGet("user-profile/{id}/{askerId}")]
        public async Task<IActionResult> GetUserProfileModel([FromRoute]string id, [FromRoute]string askerId)
        {
            var response = await _service.GetUserProfileModelAsync(id,askerId);

            return Ok(response);
        }

        [HttpGet("search-users/{query}/{userId}")]
        public async Task<IActionResult> SearchUsers([FromRoute]string query,[FromRoute] string userId)
        {
            var response = await _service.SearchUsers(query,userId);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> UserSignUp([FromBody]CreateUserDto dto)
        {
            await _service.CreateUser(dto);

            return Ok("User Created Succesfully");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> userLogIn([FromBody]UserLogInDto dto)
        {
            var userId = await _service.LogIn(dto);

            return Ok(userId);
        }

        
        [HttpPut("SetOnline/{id}")]
        public async Task<IActionResult> SetOnline([FromRoute]int id)
        {
            await _service.SetOnlineStatus(id);

            return Ok("Status set to online");
        }

        [HttpPut("SetOffline/{id}")]
        public async Task<IActionResult> SetOffline([FromRoute] int id)
        {
            await _service.SetOfflineStatus(id);

            return Ok("Status set to offline");
        }

        [HttpPut("change-username/{id}")]
        public async Task<IActionResult> ChangeUsername([FromRoute]int id, [FromBody] string value)
        {
            await _service.ChangeUsername(id,value);
            return NoContent();
        }

        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromRoute] int id, [FromBody] string value)
        {
            await _service.ChangeEmail(id,value);
            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] string value)
        {
            await _service.ChangePassword(id,value);
            return NoContent();
        }

    }
}
