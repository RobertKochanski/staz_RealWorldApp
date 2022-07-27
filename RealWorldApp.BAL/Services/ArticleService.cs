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
        private readonly IUserRepositorie _userRepositorie;
        private readonly ITagService _tagService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IArticleRepositorie articleRepositorie, UserManager<User> userManager, IMapper mapper, ILogger<ArticleService> logger,
            IUserRepositorie userRepositorie, ITagService tagService)
        {
            _articleRepositorie = articleRepositorie;
            _userRepositorie = userRepositorie;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _tagService = tagService;
        }

        public async Task<ArticleResponseModelContainer> AddArticle(CreateUpdateArticleModelContainer addModel, ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            var tags = await _tagService.AddTag(addModel.Article.TagList);

            var article = new Article
            {
                Title = addModel.Article.Title,
                Description = addModel.Article.Description,
                Body = addModel.Article.Body,
                TagList = tags,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Author = user,
                Slug = Slug.GenerateSlug(addModel.Article.Title),
            };

            await _articleRepositorie.AddArticle(article);

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(user);
            articleMapped.TagList = addModel.Article.TagList;

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }

        public async Task<ArticleResponseModelContainerList> GetArticles(string? author, string? favorited, string? tag, int limit, int offset, ClaimsPrincipal claims)
        {
            User user = (User)null;
            List<Article> articlesList = new List<Article>();
            var actualUser = await _userRepositorie.GetUserById(claims.Identity.Name);


            if (!string.IsNullOrEmpty(tag))
            {
                var allArticle = await _articleRepositorie.GetAllArticleForTag();
                foreach (var article in allArticle)
                {
                    foreach (var articleTag in article.TagList)
                    {
                        if (articleTag.Name == tag)
                        {
                            articlesList.Add(article);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(favorited))
            {
                user = await _userManager.FindByNameAsync(favorited);
                foreach (var item in user.FavoriteArticles)
                {
                    articlesList.Add(await _articleRepositorie.GetArticleBySlug(item.Slug));
                }
            }
            else if (!string.IsNullOrEmpty(author))
            {
                user = await _userManager.FindByNameAsync(author);
                articlesList = await _articleRepositorie.GetAllArticleForUser(user);
            }
            else
            {
                articlesList = await _articleRepositorie.GetAllArticle();
            }

            var sortedList = articlesList.OrderByDescending(article => article.FavoritesCount).ToList();
            var resultToMap = sortedList.Skip(offset).Take(limit);
            
            if (actualUser != null)
            {
                foreach (var item in resultToMap)
                {
                    item.Favorited = actualUser.FavoriteArticles.Contains(item);
                }
            }
            else
            {
                foreach (var item in resultToMap)
                {
                    item.Favorited = false;
                }
            }

            var result = _mapper.Map<List<ArticleResponseModel>>(resultToMap);
            
            ArticleResponseModelContainerList articleContainerList = new ArticleResponseModelContainerList() { Articles = result };
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
                _logger.LogError("Can't find article with this slug!");
                throw new BadRequestException("Can't find article with this slug!");
            }

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };
            if (actualUser != null && actualUser.FollowedUsers.Contains(author))
            {
                articleContainer.Article.Author.Following = true;
            }

            return articleContainer;
        }

        public async Task<ArticleResponseModelContainerList> GetArticleFeed(int limit, int offset, ClaimsPrincipal claims)
        {
            var actualUser = await _userRepositorie.GetUserById(claims.Identity.Name);

            var followedUsersArticles = new List<Article>();

            foreach (var user in actualUser.FollowedUsers)
            {
                followedUsersArticles.AddRange(await _articleRepositorie.GetAllArticleForUser(user));
            }

            var sortedList = followedUsersArticles.OrderByDescending(article => article.FavoritesCount).ToList();
            var result = sortedList.Skip(offset).Take(limit);

            if (actualUser == null)
            {
                foreach (var item in result)
                {
                    item.Favorited = false;
                }
            }
            else
            {
                foreach (var item in result)
                {
                    item.Favorited = actualUser.FavoriteArticles.Contains(item);
                }
            }

            ArticleResponseModelContainerList articleContainerList = new ArticleResponseModelContainerList() { Articles = _mapper.Map<List<ArticleResponseModel>>(result) };
            articleContainerList.ArticlesCount = followedUsersArticles.Count;

            return articleContainerList;
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

            var checkTags = await _tagService.CheckTags();
            await _tagService.RemoveTag(checkTags);
        }

        public async Task<ArticleResponseModelContainer> UpdateArticle(string slug, CreateUpdateArticleModelContainer updateModel)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);
            var tags = await _tagService.AddTag(updateModel.Article.TagList);

            if (article == null)
            {
                _logger.LogError("Can't update this article!");
                throw new BadRequestException("Can't update this article!");
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

            article.TagList = tags;
            article.UpdateDate = DateTime.Now;

            await _articleRepositorie.SaveChangesAsync(article);

            var checkTags = await _tagService.CheckTags();
            await _tagService.RemoveTag(checkTags);

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);
            articleMapped.TagList = updateModel.Article.TagList;

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }

        public async Task<ArticleResponseModelContainer> AddFavorite(string slug, ClaimsPrincipal claims)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user != null && article != null)
            {
                user.FavoriteArticles.Add(article);
                await _userManager.UpdateAsync(user);

                article.FavoritesCount++;
                await _articleRepositorie.SaveChangesAsync(article);
            }
            
            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);
            articleMapped.FavoritesCount++;

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }

        public async Task<ArticleResponseModelContainer> UnFavorite(string slug, ClaimsPrincipal claims)
        {
            var article = await _articleRepositorie.GetArticleBySlug(slug);
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user == null || article == null)
            {
                throw new BadRequestException("");
            }

            user.FavoriteArticles.Remove(article);
            await _userManager.UpdateAsync(user);

            article.FavoritesCount--;
            await _articleRepositorie.SaveChangesAsync(article);

            var articleMapped = _mapper.Map<ArticleResponseModel>(article);
            articleMapped.Author = _mapper.Map<UserArticleResponseModel>(article.Author);
            articleMapped.Favorited = false;

            ArticleResponseModelContainer articleContainer = new ArticleResponseModelContainer() { Article = articleMapped };

            return articleContainer;
        }
    }
}
