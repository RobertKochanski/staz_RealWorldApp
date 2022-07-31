using AutoMapper;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.ArticleServiceTests
{
    public class GetArticleFeedTests
    {
        [Category("GetArticleFeed")]
        [Test]
        public async Task GetArticleFeed_WithCorrectData_ReturnArticleResponseModelContainerList()
        {
            // ARRANGE
            var actualUser = new User()
            {
                UserName = "Username"
            };

            var articleList = new List<Article>()
            {
                new Article()
                {
                    Title = "art1"
                },
                new Article()
                {
                    Title = "art2"
                }
            };

            List<ArticleResponseModel> articleResponseList = new List<ArticleResponseModel>
            {
                new ArticleResponseModel
                {
                    Title = "art1"
                },
                new ArticleResponseModel
                {
                    Title = "art2"
                },
            };

            ArticleResponseModelContainerList expect = new ArticleResponseModelContainerList()
            {
                Articles = new List<ArticleResponseModel> {
                    new ArticleResponseModel
                    {
                        Title = "art1"
                    },
                    new ArticleResponseModel
                    {
                        Title = "art2"
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
            mockUserRepo.Setup(x => x.GetUserById(It.IsAny<string>())).ReturnsAsync(actualUser);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetAllArticleForFollowedUser(It.IsAny<User>())).ReturnsAsync(articleList);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<ArticleResponseModel>>(articleList)).Returns(articleResponseList);

            var articleService = new ArticleService(mockArticleRepo.Object, null, mockMapper.Object, null, mockUserRepo.Object, null);

            // ACT
            var result = await articleService.GetArticleFeed(5, 0, claimsPrincipal);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.ArticlesCount, Is.EqualTo(result.ArticlesCount));
            Assert.That(expect.Articles[0].Title, Is.EqualTo(result.Articles[0].Title));
        }
    }
}
