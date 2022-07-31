using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Models.UserModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.UserServiceTests
{
    public class GetUserProfileTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("GetProfile")]
        [Test]
        public async Task GetMyProfile_WithCorrectData_ReturnProfileResponseContainer()
        {
            //Arrange
            User user = new User
            {
                UserName = "tester",
                Bio = "Bio",
                Image = "Image.jpg",
            };

            ProfileResponse response = new ProfileResponse()
            {
                Username = "tester",
                Bio = "Bio",
                Image = "Image.jpg",
            };

            ProfileResponseContainer expected = new ProfileResponseContainer()
            {
                Profile = new ProfileResponse
                {
                    Username = "tester",
                    Bio = "Bio",
                    Image = "Image.jpg",
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
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ProfileResponse>(It.IsAny<User>())).Returns(response);

            var userService = new UserService(mockMapper.Object, null, mockUserManager.Object, null);

            //Act
            var actual = await userService.GetProfile("tester", claimsPrincipal);

            //Assert
            Assert.IsNotNull(actual);
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
            Assert.That(actual, Is.InstanceOf(expected.GetType()));
        }

        [Category("GetProfile")]
        [Test]
        public async Task GetMyProfile_WithIncorrectData_ThrowBadRequestException()
        {
            //Arrange
            User user = null;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Email, "tester@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't get your profile"), async delegate { await userService.GetProfile("Username", claimsPrincipal); });
        }
    }
}
