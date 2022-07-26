using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IArticleRepositorie
    {
        Task AddArticle(Article article);
        Task<List<Article>> GetAllArticle();
        Task<List<Article>> GetAllArticleForUser(User user);
        Task<List<Article>> GetAllArticleForTag();
        Task<Article> GetArticleBySlug(string slug);
        void DeleteArticle(Article article);
        Task SaveChangesAsync(Article article);
    }
}
