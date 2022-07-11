using RealWorldApp.BAL.Models;

namespace RealWorldApp.BAL.Services.Intefaces
{
    public interface IUserService
    {
        Task<List<ViewUserModel>> GetUsers();
        Task<ViewUserModel> GetUserByUsername(string Username);
        Task<ViewUserModel> GetUserById(string Id);
        Task UpdateUser(string id, UserUpdateModel request);
        Task AddUser(UserRegister request);
        Task<string> GenerateJwt(UserLogin model);
    }
}
