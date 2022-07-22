using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using RealWorldApp.Commons.Utils;
using System.Security.Claims;

namespace RealWorldApp.BAL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepositorie _articleRepositorie;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IArticleRepositorie articleRepositorie, UserManager<User> userManager, IMapper mapper, ILogger<ArticleService> logger)
        {
            _articleRepositorie = articleRepositorie;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ArticleResponseModelContainer> AddArticle(CreateUpdateArticleModelContainer addModel, ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            var article = new Article
            {
                Title = addModel.Article.Title,
                Description = addModel.Article.Description,
                Body = addModel.Article.Body,
                // TagList = new List<Tag>(addModel.Article.TagList), odkomentować po dodaniu funkcjonalności dla tagów!!!!
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Author = user,
                Slug = Slug.GenerateSlug(addModel.Article.Title),
            };

            await _articleRepositorie.AddArticle(article);

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(user);

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }

        public async Task<ArticleResponseModelContainerList> GetArticles(string? author, string? favorited, int limit, int offset)
        {
            User user = (User)null;
            List<Article> articlesList = new List<Article>();

            if (author != null)
            {
                user = await _userManager.FindByNameAsync(author);
            }

            if (favorited != null)
            {
                //articlesList = await _articleRepositorie.GetAllFavoritedArticle(limit, offset);
            }
            else if (user != null)
            {
                articlesList = await _articleRepositorie.GetAllArticleForUser(user, limit, offset);
            }
            else
            {
                articlesList = await _articleRepositorie.GetAllArticle(limit, offset);
            }

            ArticleResponseModelContainerList articleContainerList = new ArticleResponseModelContainerList() { Articles = _mapper.Map<List<ArticleResponseModel>>(articlesList) };
            articleContainerList.ArticlesCount = articlesList.Count;

            return articleContainerList;
        }

        public async Task<ArticleResponseModelContainer> GetArticleBySlug(string slug, ClaimsPrincipal claims)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);
            var author = await _userManager.FindByNameAsync(article.Author.UserName);
            var actualUser = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (article == null)
            {
                _logger.LogError("Can't find article with this slug! :(");
                throw new BadRequestException("Can't find article with this slug! :(");
            }

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            if (actualUser.FollowedUsers.Contains(author))
            {
                articleContainer.Article.Author.Following = true;
            }

            return articleContainer;
        }

        public async Task DeleteArticle(string slug)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);

            if (article == null)
            {
                _logger.LogError("Can't delete this article!");
                throw new BadRequestException("Can't delete this article!");
            }

            _articleRepositorie.DeleteArticle(article);
        }

        public async Task<ArticleResponseModelContainer> UpdateArticle(string slug, CreateUpdateArticleModelContainer updateModel)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);

            if (article == null)
            {
                _logger.LogError("Can't delete this article!");
                throw new BadRequestException("Can't delete this article!");
            }

            if (!string.IsNullOrEmpty(updateModel.Article.Title))
            {
                article.Title = updateModel.Article.Title;
                article.Slug = Slug.GenerateSlug(updateModel.Article.Title);
            }

            if (!string.IsNullOrEmpty(updateModel.Article.Description))
            {
                article.Description = updateModel.Article.Description;
            }

            if (!string.IsNullOrEmpty(updateModel.Article.Body))
            {
                article.Body = updateModel.Article.Body;
            }

            article.UpdateDate = DateTime.Now;

            await _articleRepositorie.SaveChangesAsync();

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }
    }
}
