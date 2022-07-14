using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IUserRepositorie
    {
        Task<List<User>> GetUsers();
        Task AddUser(User user);
        Task<User> GetUserByEmail(string Email);
        Task<User> GetUserById(string Id);
        Task<User> GetUserByUsername(string Username);
        Task SaveChangesAsync();
    }
}
