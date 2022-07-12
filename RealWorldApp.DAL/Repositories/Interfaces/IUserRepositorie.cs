using RealWorldApp.DAL.Entities;

namespace RealWorldApp.DAL.Repositories.Interfaces
{
    public interface IUserRepositorie
    {
        Task<List<User>> GetUsers();
        Task AddUser(User user);
        Task<User> GetUserByEmail(string Email);
        Task<User> GetUserById(string Id);
        Task SaveChangesAsync();
    }
}
