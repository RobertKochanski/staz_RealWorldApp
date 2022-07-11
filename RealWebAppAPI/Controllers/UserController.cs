using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldApp.BAL.Models;
using RealWorldApp.BAL.Services.Intefaces;

namespace RealWebAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserRegister request)
        {
            await _userService.AddUser(request);
            return Ok();
        }

        [HttpPost("authenticate")]
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
        public async Task<IActionResult> GetUserByUsername(string Username)
        {
            return Ok(await _userService.GetUserByUsername(Username));
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
