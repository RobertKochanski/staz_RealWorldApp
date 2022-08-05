using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.UserModel;

namespace RealWorldApp.Tests.UnitTests.UserServiceTests
{
    public class GenerateJWTTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("GenerateJwt")]
        [Test]
        public async Task GenerateToken_WithCorrectData_ReturnToken()
        {
            //Arrange
            UserLogin userLogin = new UserLogin
            {
                Email = "test@test.com",
                Password = "zaq1@WSX"
            };

            User user = new User
            {
                Email = "test@test.com",
                PasswordHash = "HashedPassword"
            };

            AuthenticationSettings authenticationSettings = new AuthenticationSettings
            {
                JwtKey = "PRIVATE_KEY_DONT_SHARE",
                JwtExpireDays = 5,
                JwtIssuer = "http://localhost:47765"
            };

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.CheckPasswordAsync(user, userLogin.Password)).ReturnsAsync(true);

            var userService = new UserService(null, authenticationSettings, mockUserManager.Object, null);

            //Act
            var actual = await userService.GenerateJwt(userLogin.Email, userLogin.Password);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Category("GenerateJwt")]
        [Test]
        public async Task GenerateToken_WithIncorrectUsername_ThrowBadRequestException()
        {
            //Arrange
            UserLogin userLogin = new UserLogin
            {
                Email = "test@test.com",
                Password = "zaq1@WSX"
            };

            User user = null;

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Invalid username or password"), async delegate { await userService.GenerateJwt("tester@tester.com", "haslo"); });
        }

        [Category("GenerateJwt")]
        [Test]
        public async Task GenerateToken_WithIncorrectPassword_ThrowBadRequestException()
        {
            //Arrange
            UserLogin userLogin = new UserLogin
            {
                Email = "test@test.com",
                Password = "zaq1@WSX"
            };

            User user = new User
            {
                Email = "test@test.com",
                PasswordHash = "HashedPassword"
            };

            Mock<IUserRepositorie> mockRepositorie = new Mock<IUserRepositorie>();
            mockRepositorie.Setup(x => x.GetUserByEmail(userLogin.Email)).ReturnsAsync(user);

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.CheckPasswordAsync(user, userLogin.Password));

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Invalid username or password"), async delegate { await userService.GenerateJwt("tester@tester.com", "haslo"); });
        }
    }
}
