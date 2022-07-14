using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models;

namespace RealWebAppAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("users")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegisterContainer model)
        {
            UserResponseContainer user = await _userService.AddUser(model.User);
            string token = await _userService.GenerateJwt(model.User.Email, model.User.Password);

            user.User.Token = token;

            return Ok(user);
        }

        [AllowAnonymous]
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
            UserResponseContainer user = await _userService.GetMyInfo(User);

            if (user.User.Token != null)
            {
                string token = this.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                user.User.Token = token;
            }

            return Ok(user);
        }

        [HttpGet("profiles/{Username}")]
        public async Task<IActionResult> GetProfile([FromRoute]string Username)
        {
            return Ok(await _userService.GetProfile(Username));
        }

        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser(UserUpdateModelContainer request)
        {
            string token = this.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            UserResponseContainer user = await _userService.UpdateUser(request, User, token);
            return Ok(user);
        }
    }
}
