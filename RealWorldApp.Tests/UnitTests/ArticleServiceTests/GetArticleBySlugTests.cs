using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.UnitTests.ArticleServiceTests
{
    public class GetArticleBySlugTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("GetArticleBySlug")]
        [Test]
        public async Task GetArticleBySlug_CorrectSlug_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            var author = new User()
            {
                UserName = "Username"
            };
            var actualUser = new User()
            {
                UserName = "Username"
            };
            var articleResponse = new ArticleResponseModel()
            {
                Title = "title",
                Description = "desc",
                Body = "body",
                Author = new UserArticleResponseModel(),
            };

            var article = new Article()
            {
                Title = "title",
                Description = "desc",
                Body = "body",
                Author = author,
            };

            ArticleResponseModelContainer expect = new ArticleResponseModelContainer()
            {
                Article = new ArticleResponseModel()
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body",
                    Author = new UserArticleResponseModel(),
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

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(author);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(actualUser);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ArticleResponseModel>(article)).Returns(articleResponse);
            mockMapper.Setup(x => x.Map<UserArticleResponseModel>(It.IsAny<User>())).Returns(It.IsAny<UserArticleResponseModel>);

            var articleService = new ArticleService(mockArticleRepo.Object, userManager.Object, mockMapper.Object, null, null, null);

            // ACT
            var result = await articleService.GetArticleBySlug("slug", claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Article.Title, Is.EqualTo(result.Article.Title));
        }

        [Category("GetArticleBySlug")]
        [Test]
        public async Task GetArticleBySlug_IncorrectSlug_ThrowBadRequestException()
        {
            // ARRANGE
            var author = new User()
            {
                UserName = "Username"
            };
            var actualUser = new User()
            {
                UserName = "Username"
            };
            var articleResponse = new ArticleResponseModel()
            {
                Title = "title",
                Description = "desc",
                Body = "body",
                Author = new UserArticleResponseModel(),
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync((Article)null);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(author);
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(actualUser);

            Mock<ILogger<ArticleService>> mockLogger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(mockArticleRepo.Object, userManager.Object, null, mockLogger.Object, null, null);

            // ACT

            // ASSERT
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't find article with this slug!"), async delegate { await articleService.GetArticleBySlug("slug", claimsPrincipal); });
        }
    }
}
