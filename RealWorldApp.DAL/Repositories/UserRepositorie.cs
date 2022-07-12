using Microsoft.EntityFrameworkCore;
using RealWebAppAPI.Data;
using RealWorldApp.DAL.Entities;
using RealWorldApp.DAL.Repositories.Interfaces;

namespace RealWorldApp.DAL.Repositories
{
    public class UserRepositorie : IUserRepositorie
    {
        private readonly ApplicationDbContext _context;

        public UserRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.users.ToListAsync();
        }

        public async Task<User> GetUserByEmail(string Email)
        {
            return await _context.users.Where(x => x.Email == Email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(string Id)
        {
            return await _context.users.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task AddUser(User user)
        {
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
