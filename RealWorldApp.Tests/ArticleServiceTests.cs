using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Tests
{
    public class ArticleServiceTests
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
            mockArticleRepo.Setup(x => x.AddArticle(It.IsAny<Article>()));

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ArticleResponseModel>(It.IsAny<Article>())).Returns(article);
            mockMapper.Setup(x => x.Map<UserArticleResponseModel>(It.IsAny<User>())).Returns(It.IsAny<UserArticleResponseModel>);

            var userService = new ArticleService(mockArticleRepo.Object, userManager.Object, mockMapper.Object, null, null, mockTagService.Object);

            // ACT
            var result = await userService.AddArticle(model, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect, Is.InstanceOf(result.GetType()));
            Assert.That(expect.Article.Title, Is.EqualTo(result.Article.Title));
        }

        [Category("GetArticles")]
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
           
            var userService = new ArticleService(mockArticleRepo.Object, null, mockMapper.Object, null, mockUserRepo.Object, null);

            // ACT
            var result = await userService.GetArticles(null, null, null, 10, 0, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.ArticlesCount, Is.EqualTo(result.ArticlesCount));
            Assert.That(expect.Articles.GetType(), Is.EqualTo(result.Articles.GetType()));
        }

        [Category("GetArticles")]
        [Test]
        public async Task GetArticlesForAuthor_ReturnArticleResponseModelContainerList()
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
            mockArticleRepo.Setup(x => x.GetAllArticleForUser(user)).ReturnsAsync(articlesList);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<ArticleResponseModel>>(articlesList)).Returns(articleResponseList);

            var userService = new ArticleService(mockArticleRepo.Object, userManager.Object, mockMapper.Object, null, mockUserRepo.Object, null);

            // ACT
            var result = await userService.GetArticles("Username", null, null, 10, 0, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.ArticlesCount, Is.EqualTo(result.ArticlesCount));
            Assert.That(expect.Articles.GetType(), Is.EqualTo(result.Articles.GetType()));
        }
    }
}
