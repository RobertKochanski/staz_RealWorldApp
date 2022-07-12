using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldApp.BAL.Models;
using RealWorldApp.BAL.Services.Intefaces;

namespace RealWebAppAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserRegister request)
        {
            await _userService.AddUser(request);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate(UserLogin model)
        {
            string token = await _userService.GenerateJwt(model);

            return Ok(token);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("Username")]
        public async Task<IActionResult> GetUserByEmail(string Email)
        {
            return Ok(await _userService.GetUserByEmail(Email));
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetUserById(string Id)
        {
            return Ok(await _userService.GetUserById(Id));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(string id, UserUpdateModel request)
        {
            await _userService.UpdateUser(id, request);
            return Ok();
        }
    }
}
