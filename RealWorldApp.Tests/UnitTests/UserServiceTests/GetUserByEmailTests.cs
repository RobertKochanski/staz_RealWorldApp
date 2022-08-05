using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Models.UserModel;

namespace RealWorldApp.Tests.UnitTests.UserServiceTests
{
    public class GetUserByEmailTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("GetUserByEmail")]
        [Test]
        public async Task GetUserByEmail_WithCorrectData_ReturnUserResponseContainer()
        {
            //Arrange
            User user = new User
            {
                UserName = "tester",
                Email = "tester@tester.com",
            };

            UserResponse response = new UserResponse()
            {
                Username = "tester",
                Email = "tester@tester.com"
            };

            UserResponseContainer expected = new UserResponseContainer()
            {
                User = new UserResponse
                {
                    Username = "tester",
                    Email = "tester@tester.com",
                }
            };

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(mockMapper.Object, null, mockUserManager.Object, mockLogger.Object);

            //Act
            var actual = await userService.GetUserByEmail("tester@tester.com");

            //Assert
            Assert.IsNotNull(actual);
            Assert.That(actual.User.Username, Is.EqualTo(expected.User.Username));
            Assert.That(actual.User.Email, Is.EqualTo(expected.User.Email));
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
            Assert.That(actual, Is.InstanceOf(expected.GetType()));
        }

        [Category("GetUserByEmail")]
        [Test]
        public async Task GetUserByEmail_WithIncorrectData_ThrowBadRequestException()
        {
            //Arrange
            User user = null;

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't find user"), async delegate { await userService.GetUserByEmail("tezter@tester.com"); });
        }
    }
}
