using Microsoft.EntityFrameworkCore;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;

namespace RealWorldApp.DAL.Repositories
{
    public class TagRepositorie : ITagRepositorie
    {
        private readonly ApplicationDbContext _context;

        public TagRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> AddTag(Tag tag)
        {
            await _context.tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> GetTag(string name)
        {
            return await _context.tags.Include(x => x.Articles).Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Tag>> GetTags()
        {
            return await _context.tags.Include(x => x.Articles).ToListAsync();
        }

        public void RemoveTag(Tag tag)
        {
            _context.Remove(tag);
            _context.SaveChanges();
        }
    }
}
