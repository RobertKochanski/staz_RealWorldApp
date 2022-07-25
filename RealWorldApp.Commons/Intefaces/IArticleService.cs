using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Commons.Intefaces
{
    public interface IArticleService
    {
        Task<ArticleResponseModelContainer> AddArticle(CreateUpdateArticleModelContainer addModel, ClaimsPrincipal claims);
        Task<ArticleResponseModelContainerList> GetArticles(string? author, string? favorited, int limit, int offset, ClaimsPrincipal claims);
        Task<ArticleResponseModelContainer> GetArticleBySlug(string slug, ClaimsPrincipal claims);
        Task<ArticleResponseModelContainerList> GetArticleFeed(int limit, int offset, ClaimsPrincipal claims);
        Task DeleteArticle(string slug);
        Task<ArticleResponseModelContainer> UpdateArticle(string slug, CreateUpdateArticleModelContainer updateModel);
        Task<ArticleResponseModelContainer> AddFavorite(string slug, ClaimsPrincipal claims);
        Task<ArticleResponseModelContainer> UnFavorite(string slug, ClaimsPrincipal claims);
    }
}
