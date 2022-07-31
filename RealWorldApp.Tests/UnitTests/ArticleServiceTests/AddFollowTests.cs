using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.ArticleServiceTests
{
    public class AddFollowTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("AddFollow")]
        [Test]
        public async Task AddFollow_WithCorrectData_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            var article = new Article
            {
                Slug = "title",
                Title = "title",
                Favorited = false,
                FavoritesCount = 0,
            };
            var user = new User
            {
                UserName = "username",
                FavoriteArticles = new List<Article>()
                {
                    article,
                }
            };
            var articleResponse = new ArticleResponseModel
            {
                Slug = "title",
                Title = "title",
                Favorited = true,
                FavoritesCount = 1,
            };

            var expect = new ArticleResponseModelContainer
            {
                Article = new ArticleResponseModel
                {
                    Slug = "title",
                    Title = "title",
                    Favorited = true,
                    FavoritesCount = 1,
                }
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(article);
            mockArticleRepo.Setup(x => x.SaveChangesAsync(article));

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.UpdateAsync(user));

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ArticleResponseModel>(article)).Returns(articleResponse);
            mockMapper.Setup(x => x.Map<UserArticleResponseModel>(article.Author)).Returns(articleResponse.Author);

            var articleService = new ArticleService(mockArticleRepo.Object, mockUserManager.Object, mockMapper.Object, null, null, null);

            // ACT
            var result = await articleService.AddFavorite("slug", claimsPrincipal);

            // ASSERT
            Assert.IsTrue(result.Article.Favorited);
            Assert.That(expect.Article.Favorited, Is.EqualTo(result.Article.Favorited));
            Assert.That(expect.Article.FavoritesCount, Is.EqualTo(result.Article.FavoritesCount)); 
        }
    }
}
