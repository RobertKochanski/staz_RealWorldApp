using RealWorldApp.Commons.Models;
using System.Security.Claims;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IUserService
    {
        Task<List<ViewUserModel>> GetUsers();
        Task<UserResponseContainer> GetUserByEmail(string Email);
        Task<UserResponseContainer> UpdateUser(UserUpdateModelContainer request, ClaimsPrincipal claims);
        Task<UserResponseContainer> AddUser(UserRegister request);
        Task<string> GenerateJwt(string Email, string Password);
        Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims);
        Task<ProfileResponseContainer> GetProfile(string Username);
    }
}

