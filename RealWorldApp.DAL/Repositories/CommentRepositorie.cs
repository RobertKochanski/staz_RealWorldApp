using Microsoft.EntityFrameworkCore;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.CommentModel;

namespace RealWorldApp.DAL.Repositories
{
    public class CommentRepositorie : ICommentRepositorie
    {
        private readonly ApplicationDbContext _context;

        public CommentRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddComment(Comment comment)
        {
            await _context.comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetCommentsForArticle(string slug)
        {
            return await _context.comments
                .Include(x => x.Article)
                .Include(x => x.Author)
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.Article.Slug == slug)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentById(int id)
        {
            return await _context.comments
                .Include(x => x.Article)
                .Include(x => x.Author)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public void DeleteComment(Comment comment)
        {
            _context.Remove(comment);
            _context.SaveChanges();
        }
    }
}
