using Microsoft.AspNetCore.Mvc;
using Foodiefeed_api.services;
using Foodiefeed_api.models.user;

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

        [HttpPost("register")]
        public async Task<ActionResult> UserSignUp([FromBody]CreateUserDto dto)
        {
            await _service.CreateUser(dto);

            return Ok("User Created Succesfully");
        }

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
      
    }
}
