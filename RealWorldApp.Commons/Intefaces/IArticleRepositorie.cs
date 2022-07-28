using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IArticleRepositorie
    {
        Task AddArticle(Article article);
        Task<List<Article>> GetAllArticle();
        Task<List<Article>> GetAllArticleForAuthor(User user);
        Task<List<Article>> GetAllFavoritedArticles(User user);
        Task<List<Article>> GetAllArticleForFollowedUser(User user);
        Task<List<Article>> GetAllArticleForTag(string tag);
        Task<Article> GetArticleBySlug(string slug);
        void DeleteArticle(Article article);
        Task SaveChangesAsync(Article article);
    }
}
