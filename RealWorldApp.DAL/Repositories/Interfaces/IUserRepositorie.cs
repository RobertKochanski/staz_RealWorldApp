using RealWorldApp.DAL.Entities;

namespace RealWorldApp.DAL.Repositories.Interfaces
{
    public interface IUserRepositorie
    {
        Task<List<User>> GetUsers();
        Task AddUser(User user);
        Task<User> GetUserByUsername(string Username);
        Task<User> GetUserById(string Id);
        Task SaveChangesAsync();
    }
}
