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
    public class UserRegisterTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("RegisterUser")]
        [Test]
        public async Task Register_WithCorrectData_ReturnUserAsync()
        {
            //Arrange
            UserRegister user = new UserRegister()
            {
                Username = "tester",
                Email = "tester@tester.com",
                Password = "zaq1@WSX"
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

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(It.IsAny<User>());
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(mockMapper.Object, null, userManager.Object, null);

            //Act
            var actual = await userService.AddUser(user);

            //Assert
            Assert.IsNotNull(actual);
            mockMapper.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Once());
            Assert.That(actual.User.Username, Is.EqualTo(expected.User.Username));
        }

        [Category("RegisterUser")]
        [Test]
        public async Task Register_WithIncorrectData_ThrowBadRequestException()
        {
            //Arrange
            UserRegister user = new UserRegister()
            {
                Username = "ttest",
                Email = "ttest@test.com",
                Password = "test"
            };

            IdentityResult result = new IdentityResult();

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, userManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo(string.Join(" ", result.Errors.Select(x => x.Description))), async delegate { await userService.AddUser(user); });
        }
    }
}
