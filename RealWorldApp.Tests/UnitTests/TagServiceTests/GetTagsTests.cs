using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.TagModel;

namespace RealWorldApp.Tests.TagServiceTests
{
    public class GetTagsTests
    {
        [Category("GetTags")]
        [Test]
        public async Task GetTagsList_WithCorrectData_ReturnTagResponseModel()
        {
            // Arrange
            var tagList = new List<Tag>
            {
                new Tag{ Name = "tag1" },
                new Tag{ Name = "tag2" },
            };

            var expect = new TagResponseModel
            {
                Tags = new List<string>
                {
                    "tag1",
                    "tag2",
                }
            };

            Mock<ITagRepositorie> mockTagRepo = new Mock<ITagRepositorie>();
            mockTagRepo.Setup(x => x.GetTags()).ReturnsAsync(tagList);

            var tagService = new TagService(mockTagRepo.Object);

            // Act
            var result = await tagService.GetTags();

            // Assert 
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Tags.Count, Is.EqualTo(result.Tags.Count));
            Assert.That(expect.Tags[0], Is.EqualTo(result.Tags[0]));
        }
    }
}
