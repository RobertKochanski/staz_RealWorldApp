using Microsoft.EntityFrameworkCore;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;

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
            return await _context.users.Where(x => x.Id == Id).Include(x => x.FollowedUsers).Include(x => x.FavoriteArticles).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsername(string Username)
        {
            return await _context.users.Where(x => x.UserName == Username).FirstOrDefaultAsync();
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
