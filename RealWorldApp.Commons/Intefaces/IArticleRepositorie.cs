using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IArticleRepositorie
    {
        Task AddArticle(Article article);
        Task<List<Article>> GetAllArticle(int limit, int offset);
        Task<List<Article>> GetAllArticleForUser(User user, int limit, int offset);
        Task<Article> GetArticleBySlug(string slug);
        void DeleteArticle(Article article);
        Task SaveChangesAsync(Article article);
    }
}
