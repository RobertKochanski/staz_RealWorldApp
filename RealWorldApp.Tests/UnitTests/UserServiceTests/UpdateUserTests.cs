using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Models.UserModel;
using System.Security.Claims;

namespace RealWorldApp.Tests.UnitTests.UserServiceTests
{
    public class UpdateUserTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Category("UpdateUser")]
        [Test]
        public async Task UpdateUser_WithCorrectData_ReturnUserResponseContainer()
        {
            //Arrange
            User userToChange = new User
            {
                Email = "test@test.com",
                PasswordHash = "HashedPassword",
                Bio = "ShortBio",
                Image = "Image",
            };

            UserUpdateModelContainer updateModel = new UserUpdateModelContainer()
            {
                User = new UserUpdateModel
                {
                    UserName = "tester",
                    Email = "tester@test.com",
                    Bio = "ShortBios",
                    Image = "Image.jpg",
                }
            };

            UserResponse response = new UserResponse
            {
                Username = "tester",
                Email = "tester@test.com",
                Bio = "ShortBios",
                Image = "Image.jpg",
            };

            UserResponseContainer expected = new UserResponseContainer()
            {
                User = new UserResponse
                {
                    Username = "tester",
                    Email = "tester@test.com",
                    Bio = "ShortBios",
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
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userToChange);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            Mock<IPasswordHasher<User>> mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(userToChange.PasswordHash);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            var userService = new UserService(mockMapper.Object, null, mockUserManager.Object, null);

            //Act
            var actual = userService.UpdateUser(updateModel, claimsPrincipal);

            //Assert
            Assert.IsTrue(expected.GetType() == actual.Result.GetType());
            Assert.That(actual.Result.User.Email, Is.EqualTo(expected.User.Email));
            Assert.That(actual.Result.User.Username, Is.EqualTo(expected.User.Username));
            Assert.That(actual.Result.User.Bio, Is.EqualTo(expected.User.Bio));
        }

        [Category("UpdateUser")]
        [Test]
        public async Task UpdateUser_WithUserEqualNull_ThrowBadRequestException()
        {
            //Arrange
            User userToChange = null;

            UserUpdateModelContainer updateModel = new UserUpdateModelContainer()
            {
                User = new UserUpdateModel
                {
                    UserName = "tester",
                    Email = "tester@test.com",
                    Bio = "ShortBios",
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
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userToChange);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't find user"), async delegate { await userService.UpdateUser(updateModel, claimsPrincipal); });
        }

        [Category("UpdateUser")]
        [Test]
        public async Task UpdateUser_WithIncorrectData_ThrowBadRequestException()
        {
            //Arrange
            User userToChange = new User
            {
                Email = "test@test.com",
                PasswordHash = "HashedPassword",
                Bio = "ShortBio",
                Image = "Image",
            };

            UserUpdateModelContainer updateModel = new UserUpdateModelContainer()
            {
                User = new UserUpdateModel
                {
                    UserName = "tester",
                    Email = "tester@test.com",
                    Bio = "ShortBios",
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

            IdentityResult result = new IdentityResult();

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userToChange);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            Mock<IPasswordHasher<User>> mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(userToChange.PasswordHash);

            Mock<ILogger<UserService>> mockLogger = new Mock<ILogger<UserService>>();

            var userService = new UserService(null, null, mockUserManager.Object, mockLogger.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo(string.Join(" ", result.Errors.Select(x => x.Description))), async delegate { await userService.UpdateUser(updateModel, claimsPrincipal); });
        }
    }
}