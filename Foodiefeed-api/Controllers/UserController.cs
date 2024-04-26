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

        [HttpPost]
        public async Task<ActionResult<string>> createUser([FromBody]CreateUserDto dto)
        {
            await _service.CreateUser(dto);

            return Ok("User Created Succesfully");
        }


    }
}
