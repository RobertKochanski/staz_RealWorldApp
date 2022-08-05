using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;

namespace RealWorldApp.Tests.UnitTests.TagServiceTests
{
    public class AddTagTests
    {
        [Category("AddTags")]
        [Test]
        public async Task AddTags_WithCorrectData_ReturnListTag()
        {
            // Arrange
            var stringList = new List<string>
            {
                "tag1",
                "tag2",
            };

            Tag tagNull = null;
            var tag1 = new Tag { Name = "tag1", };
            var tag2 = new Tag { Name = "tag2", };

            var expect = new List<Tag>
            {
                new Tag{ Name = "tag1" },
                new Tag{ Name = "tag2" },
            };

            Mock<ITagRepositorie> mockTagRepo = new Mock<ITagRepositorie>();
            mockTagRepo.SetupSequence(x => x.GetTag(It.IsAny<string>()))
                .ReturnsAsync(tagNull)
                .ReturnsAsync(tag1)
                .ReturnsAsync(tagNull)
                .ReturnsAsync(tag2);
            mockTagRepo.Setup(x => x.AddTag(It.IsAny<Tag>()));

            var tagService = new TagService(mockTagRepo.Object);

            // Act
            var result = await tagService.AddTag(stringList);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(expect.GetType(), Is.EqualTo(result.GetType()));
            Assert.That(expect.Count, Is.EqualTo(result.Count));
            Assert.That(expect[0].Name, Is.EqualTo(result[0].Name));
        }
    }
}
