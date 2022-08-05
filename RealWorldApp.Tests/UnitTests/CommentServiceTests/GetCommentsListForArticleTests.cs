using AutoMapper;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.CommentModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.UnitTests.CommentServiceTests
{
    public class GetCommentsListForArticleTests
    {
        [Category("GetCommentsList")]
        [Test]
        public async Task GetCommentsList_WithCorrectData_ReturnCommentResponseModelContainer()
        {
            // Arrange
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            List<Comment> commentsList = new List<Comment>()
            {
                new Comment(){ Body = "body1" },
                new Comment(){ Body = "body2" },
            };

            List<CommentResponseModel> commentsListMapped = new List<CommentResponseModel>()
            {
                new CommentResponseModel{ Body = "body1" },
                new CommentResponseModel{ Body = "body2" },
            };

            var expect = new CommentResponseModelContainerList
            {
                Comments = new List<CommentResponseModel>
                {
                    new CommentResponseModel{ Body = "body1" },
                    new CommentResponseModel{ Body = "body2" },
                }
            };

            Mock<ICommentRepositorie> mockCommentRepo = new Mock<ICommentRepositorie>();
            mockCommentRepo.Setup(x => x.GetCommentsForArticle(It.IsAny<string>())).ReturnsAsync(commentsList);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<CommentResponseModel>>(commentsList)).Returns(commentsListMapped);

            var commentService = new CommentService(mockCommentRepo.Object, null, null, mockMapper.Object);

            // Act
            var result = await commentService.GetComments(It.IsAny<string>(), claimsPrincipal);

            // Assert 
            Assert.That(result, Is.Not.Null);
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Comments.Count, Is.EqualTo(result.Comments.Count));

        }
    }
}
