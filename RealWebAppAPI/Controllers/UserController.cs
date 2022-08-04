using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.UserModel;
using RealWorldApp.CQRS.Users.Commends;

namespace RealWebAppAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public UserController(IUserService userService, IMediator mediator)
        {
            _userService = userService;
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("users")]
        public async Task<IActionResult> RegisterUser(CreateUserCommand command)
        {
            
            return Ok(await _mediator.Send(command));
        }

        [AllowAnonymous]
        [HttpPost("users/login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginContainer model)
        {
            UserResponseContainer user = await _userService.GetUserByEmail(model.User.Email);
            string token = await _userService.GenerateJwt(model.User.Email, model.User.Password);

            user.User.Token = token;

            return Ok(user);
        }

        //[AllowAnonymous]
        //[HttpPost("users/login")]
        //public async Task<IActionResult> Authenticate(AuthenticateUserCommand command)
        //{
        //    return Ok(await _mediator.Send(command));
        //}

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

        [HttpPut("user")]
        public async Task<IActionResult> UpdateUser(UserUpdateModelContainer request)
        {
            return Ok(await _userService.UpdateUser(request, User));
        }

        [AllowAnonymous]
        [HttpGet("profiles/{Username}")]
        public async Task<IActionResult> GetProfile([FromRoute] string Username)
        {
            return Ok(await _userService.GetProfile(Username, User));
        }

        [HttpPost("profiles/{Username}/follow")]
        public async Task<IActionResult> AddFollow([FromRoute] string Username)
        {
            return Ok(await _userService.AddFollow(Username, User));
        }

        [HttpDelete("profiles/{Username}/follow")]
        public async Task<IActionResult> UnFollow([FromRoute] string Username)
        {
            return Ok(await _userService.UnFollow(Username, User));
        }
    }
}
