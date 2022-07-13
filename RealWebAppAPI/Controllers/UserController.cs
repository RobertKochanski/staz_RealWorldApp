using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldApp.BAL.Models;
using RealWorldApp.BAL.Services.Intefaces;

namespace RealWebAppAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("users")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegisterContainer model)
        {
            UserResponseContainer user = await _userService.AddUser(model.User);
            string token = await _userService.GenerateJwt(model.User.Email, model.User.Password);

            user.User.Token = token;

            return Ok(user);
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> Authenticate([FromBody]UserLoginContainer model)
        {
            UserResponseContainer user = await _userService.GetUserByEmail(model.User.Email);
            string token = await _userService.GenerateJwt(model.User.Email, model.User.Password);

            user.User.Token = token;

            return Ok(user);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("{Email}")]
        public async Task<IActionResult> GetUserByEmail(string Email)
        {
            return Ok(await _userService.GetUserByEmail(Email));
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetMyInfo()
        {
            return Ok(await _userService.GetMyInfo(User));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateModel request)
        {
            await _userService.UpdateUser(id, request);
            return Ok();
        }
    }
}
