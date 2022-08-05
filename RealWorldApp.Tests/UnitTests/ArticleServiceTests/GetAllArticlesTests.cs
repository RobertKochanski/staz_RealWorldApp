using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.UnitTests.ArticleServiceTests
{
    public class GetAllArticlesTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("GetAllArticles")]
        [Test]
        public async Task GetAllArticles_ReturnArticleResponseModelContainerList()
        {
            // ARRANGE
            User user = new User()
            {
                UserName = "Username"
            };

            List<ArticleResponseModel> articleResponseList = new List<ArticleResponseModel>
            {
                new ArticleResponseModel
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body"
                },
                new ArticleResponseModel
                {
                    Title = "title2",
                    Description = "desc2",
                    Body = "body2"
                },
            };

            var articlesList = new List<Article>{
                new Article
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body"
                },
                new Article
                {
                    Title = "title2",
                    Description = "desc2",
                    Body = "body2"
                },
            };

            ArticleResponseModelContainerList expect = new ArticleResponseModelContainerList()
            {
                Articles = new List<ArticleResponseModel> {
                    new ArticleResponseModel
                    {
                        Title = "title",
                        Description = "desc",
                        Body = "body"
                    },
                    new ArticleResponseModel
                    {
                        Title = "title2",
                        Description = "desc2",
                        Body = "body2"
                    },
                },
                ArticlesCount = 2
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<IUserRepositorie> mockUserRepo = new Mock<IUserRepositorie>();
            mockUserRepo.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetAllArticle()).ReturnsAsync(articlesList);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<ArticleResponseModel>>(articlesList)).Returns(articleResponseList);

            var articleService = new ArticleService(mockArticleRepo.Object, null, mockMapper.Object, null, mockUserRepo.Object, null);

            // ACT
            var result = await articleService.GetArticles(null, null, null, 10, 0, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.ArticlesCount, Is.EqualTo(result.ArticlesCount));
            Assert.That(expect.Articles.GetType(), Is.EqualTo(result.Articles.GetType()));
        }

        [Category("GetAllArticles")]
        [Test]
        public async Task GetArticlesForAuthor_WithAuthor_ReturnArticleResponseModelContainerList()
        {
            // ARRANGE
            User user = new User()
            {
                UserName = "Username"
            };

            List<ArticleResponseModel> articleResponseList = new List<ArticleResponseModel>
            {
                new ArticleResponseModel
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body"
                },
                new ArticleResponseModel
                {
                    Title = "title2",
                    Description = "desc2",
                    Body = "body2"
                },
            };

            var articlesList = new List<Article>{
                new Article
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body"
                },
                new Article
                {
                    Title = "title2",
                    Description = "desc2",
                    Body = "body2"
                },
            };

            ArticleResponseModelContainerList expect = new ArticleResponseModelContainerList()
            {
                Articles = new List<ArticleResponseModel> {
                    new ArticleResponseModel
                    {
                        Title = "title",
                        Description = "desc",
                        Body = "body"
                    },
                    new ArticleResponseModel
                    {
                        Title = "title2",
                        Description = "desc2",
                        Body = "body2"
                    },
                },
                ArticlesCount = 2
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<IUserRepositorie> mockUserRepo = new Mock<IUserRepositorie>();
            mockUserRepo.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetAllArticleForAuthor(user)).ReturnsAsync(articlesList);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<ArticleResponseModel>>(articlesList)).Returns(articleResponseList);

            var articleService = new ArticleService(mockArticleRepo.Object, userManager.Object, mockMapper.Object, null, mockUserRepo.Object, null);

            // ACT
            var result = await articleService.GetArticles("Username", null, null, 10, 0, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.ArticlesCount, Is.EqualTo(result.ArticlesCount));
            Assert.That(expect.Articles.GetType(), Is.EqualTo(result.Articles.GetType()));
        }
    }
}
