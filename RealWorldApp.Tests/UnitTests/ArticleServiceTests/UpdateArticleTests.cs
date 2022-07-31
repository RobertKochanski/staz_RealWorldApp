using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;

namespace RealWorldApp.Tests.ArticleServiceTests
{
    public class UpdateArticleTests
    {
        [Category("UpdateArticle")]
        [Test]
        public async Task UpdateArticle_WithCorrectData_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            var article = new Article
            {
                Slug = "title",
                Title = "title",
                Author = new User
                {
                    UserName = "username"
                }
            };

            var articleResponse = new ArticleResponseModel
            {
                Slug = "new-title",
                Title = "new title",
                Author = new UserArticleResponseModel
                {
                    Username = "username"
                }
            };

            var tagList = new List<Tag>()
            {
                new Tag
                {
                    Name = "1"
                },
                new Tag
                {
                    Name = "2"
                }
            };

            var updateModel = new CreateUpdateArticleModelContainer
            {
                Article = new CreateUpdateArticleModel
                {
                    Title = "new title",
                }
            };

            var expect = new ArticleResponseModelContainer
            {
                Article = new ArticleResponseModel
                {
                    Slug = "new-title",
                    Title = "new title",
                    Author = new UserArticleResponseModel
                    {
                        Username = "username"
                    }
                }
            };


            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(article);
            mockArticleRepo.Setup(x => x.SaveChangesAsync(article));

            Mock<ITagService> mockTag = new Mock<ITagService>();
            mockTag.Setup(x => x.AddTag(It.IsAny<List<string>>())).ReturnsAsync(tagList);
            mockTag.Setup(x => x.CheckTags()).ReturnsAsync(tagList);
            mockTag.Setup(x => x.RemoveTag(tagList));

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ArticleResponseModel>(article)).Returns(articleResponse);
            mockMapper.Setup(x => x.Map<UserArticleResponseModel>(article.Author)).Returns(articleResponse.Author);

            var articleService = new ArticleService(mockArticleRepo.Object, null, mockMapper.Object, null, null, mockTag.Object);

            // ACT
            var result = await articleService.UpdateArticle("title", updateModel);

            // ASSERT
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Article.Title, Is.EqualTo(result.Article.Title));

        }

        [Category("UpdateArticle")]
        [Test]
        public async Task UpdateArticle_WithIncorrectData_ReturnArticleResponseModelContainer()
        {
            // ARRANGE
            Article article = null;

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(article);

            Mock<ILogger<ArticleService>> mockLogger = new Mock<ILogger<ArticleService>>();

            var articleService = new ArticleService(mockArticleRepo.Object, null, null, mockLogger.Object, null, null);

            // ACT

            // ASSERT
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't update this article!"), async delegate { await articleService.UpdateArticle("new-title", It.IsAny<CreateUpdateArticleModelContainer>()); });
        }
    }
}
