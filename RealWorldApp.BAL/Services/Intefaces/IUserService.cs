using RealWorldApp.BAL.Models;
using System.Security.Claims;

namespace RealWorldApp.BAL.Services.Intefaces
{
    public interface IUserService
    {
        Task<List<ViewUserModel>> GetUsers();
        Task<UserResponseContainer> GetUserByEmail(string Email);
        Task UpdateUser(string id, UserUpdateModel request);
        Task<UserResponseContainer> AddUser(UserRegister request);
        Task<string> GenerateJwt(string Email, string Password);
        Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims);
    }
}

