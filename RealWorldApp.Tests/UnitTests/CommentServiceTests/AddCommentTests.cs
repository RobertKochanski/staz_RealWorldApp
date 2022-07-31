using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.CommentModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.CommentServiceTests
{
    public class AddCommentTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("AddComment")]
        [Test]
        public async Task CreateComment_WithCorrectData_ReturnCommentResponseModelContainer()
        {
            // Arrange
            var user = new User
            {
                UserName = "username",
            };

            var article = new Article
            {
                Title = "title",
                Description = "description",
                Body = "body"
            };

            var createModel = new CreateCommentModelContainer
            {
                Comment = new CreateCommentModel
                {
                    Body = "body",
                }
            };

            var commentToMap = new Comment
            {
                Body = "body",
            };

            var commentMapped = new CommentResponseModel
            {
                Body = "body",
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var expect = new CommentResponseModelContainer
            {
                Comment = new CommentResponseModel
                {
                    Body = "body"
                }
            };

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(article);

            Mock<ICommentRepositorie> mockCommentRepo = new Mock<ICommentRepositorie>();
            mockCommentRepo.Setup(x => x.AddComment(It.IsAny<Comment>()));

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CommentResponseModel>(It.IsAny<Comment>())).Returns(commentMapped);

            var commentService = new CommentService(mockCommentRepo.Object, mockUserManager.Object, mockArticleRepo.Object, mockMapper.Object);

            // Act
            var result = await commentService.AddComment(It.IsAny<string>(), createModel, claimsPrincipal);

            // Assert 
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Comment.Body, Is.EqualTo(result.Comment.Body));
        }

        [Category("AddComment")]
        [Test]
        public async Task CreateComment_WithNullArticle_ThrowBadRequestException()
        {
            // Arrange
            var user = new User
            {
                UserName = "username",
            };

            Article article = null;

            var createModel = new CreateCommentModelContainer
            {
                Comment = new CreateCommentModel
                {
                    Body = "body",
                }
            };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IArticleRepositorie> mockArticleRepo = new Mock<IArticleRepositorie>();
            mockArticleRepo.Setup(x => x.GetArticleBySlug(It.IsAny<string>())).ReturnsAsync(article);

            var commentService = new CommentService(null, mockUserManager.Object, mockArticleRepo.Object, null);

            // Act

            // Assert 
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't find article!"), async delegate { await commentService.AddComment(It.IsAny<string>(), createModel, claimsPrincipal); });
        }
    }
}
