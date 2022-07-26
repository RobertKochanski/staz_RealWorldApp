﻿using Microsoft.EntityFrameworkCore;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;

namespace RealWorldApp.DAL.Repositories
{
    public class ArticleRepositorie : IArticleRepositorie
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddArticle(Article article)
        {
            await _context.articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> GetAllArticle()
        {
            return await _context.articles
                .OrderByDescending(x => x.CreateDate)
                .Include(x => x.Author)
                .Include(x => x.TagList)
                .ToListAsync();
        }

        public async Task<List<Article>> GetAllArticleForUser(User user)
        {
            return await _context.articles
                .Include(x => x.Author)
                .Include(x => x.TagList)
                .Where(x => x.Author.UserName == user.UserName)
                .ToListAsync();
        }

        public async Task<List<Article>> GetAllArticleForTag()
        {
            return await _context.articles
                .Include(x => x.Author)
                .Include(x => x.TagList)
                .ToListAsync();
        }

        public async Task<Article> GetArticleBySlug(string slug)
        {
            return await _context.articles
                .Include(a => a.Author)
                .Include(x => x.TagList)
                .Where(x => x.Slug == slug)
                .FirstOrDefaultAsync();
        }

        public void DeleteArticle(Article article)
        {
            _context.articles.Remove(article);
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync(Article article)
        {
            _context.articles.Update(article);
            await _context.SaveChangesAsync();
        }
    }
}
