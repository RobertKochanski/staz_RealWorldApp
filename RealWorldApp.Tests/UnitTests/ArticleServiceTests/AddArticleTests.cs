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
    public class AddArticleTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("AddArticle")]
        [Test]
        public async Task CreateArticle_WithCorrectData_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            User user = new User()
            {
                UserName = "Username"
            };

            Article articleNull = null;

            ArticleResponseModel article = new ArticleResponseModel()
            {
                Title = "Title",
                Description = "Desc",
                Body = "Body"
            };

            ArticleResponseModelContainer expect = new ArticleResponseModelContainer()
            {
                Article = article
            };

            CreateUpdateArticleModelContainer model = new CreateUpdateArticleModelContainer()
            {
                Article = new CreateUpdateArticleModel()
                {
                    Title = "Title",
                    Description = "Desc",
                    Body = "Body"
                }
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<ITagService> mockTagService = new Mock<ITagService>();
            mockTagService.Setup(x => x.AddTag(It.IsAny<List<string>>())).ReturnsAsync(It.IsAny<List<Tag>>());

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(articleNull);
            mockArticleRepo.Setup(x => x.AddArticle(It.IsAny<Article>()));

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ArticleResponseModel>(It.IsAny<Article>())).Returns(article);
            mockMapper.Setup(x => x.Map<UserArticleResponseModel>(It.IsAny<User>())).Returns(It.IsAny<UserArticleResponseModel>);

            Mock<ILogger<ArticleService>> mockLogger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(mockArticleRepo.Object, userManager.Object, mockMapper.Object, mockLogger.Object, null, mockTagService.Object);

            // ACT
            var result = await articleService.AddArticle(model, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect, Is.InstanceOf(result.GetType()));
            Assert.That(expect.Article.Title, Is.EqualTo(result.Article.Title));
        }

        [Category("AddArticle")]
        [Test]
        public async Task CreateArticle_AlreadyExist_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            CreateUpdateArticleModelContainer article = new CreateUpdateArticleModelContainer
            {
                Article = new CreateUpdateArticleModel
                {
                    Title = "title",
                    Description = "desc",
                    Body = "body"
                }
            };

            Article returnBySlug = new Article
            {
                Title = "t",
                Description = "d",
                Body = "b"
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(returnBySlug);

            Mock<ILogger<ArticleService>> mockLogger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(mockArticleRepo.Object, null, null, mockLogger.Object, null, null);

            // ACT

            // ASSERT
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Article with this title already exist!"), async delegate { await articleService.AddArticle(article, claimsPrincipal); });
        }

        [Category("AddArticle")]
        [Test]
        public async Task CreateArticle_WithNullTitle_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            CreateUpdateArticleModelContainer article = new CreateUpdateArticleModelContainer
            {
                Article = new CreateUpdateArticleModel
                {
                    Title = "",
                    Description = "desc",
                    Body = "body"
                }
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<ILogger<ArticleService>> mockLogger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(null, null, null, mockLogger.Object, null, null);

            // ACT

            // ASSERT
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Please fill all required fields!"), async delegate { await articleService.AddArticle(article, claimsPrincipal); });
        }
    }
}
