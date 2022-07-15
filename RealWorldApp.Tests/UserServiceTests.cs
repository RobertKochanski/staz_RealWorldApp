using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Models;

namespace RealWorldApp.Tests
{
    public class UserServiceTests
    {
        private Mock<UserManager<User>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

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
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success); 

            var userService = new UserService(null, mockMapper.Object, null,null, userManager.Object);

            //Act
            var actual = await userService.AddUser(user);

            //Assert
            Assert.IsNotNull(actual);
            mockMapper.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Once());
            Assert.That(actual.User.Username, Is.EqualTo(expected.User.Username));
        }

        [Test]
        public async Task Register_WithUncorrectData_ThrowBadRequestException()
        {
            //Arrange
            UserRegister user = new UserRegister()
            {
                Username = "ttest",
                Email = "ttest@test.com",
                Password = "test"
            };

            Mock<UserManager<User>> userManager = GetMockUserManager();
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            var userService = new UserService(null, null, null, null, userManager.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>().And.Message.EqualTo("Something goes wrong!"), async delegate { await userService.AddUser(user); });

            var actualEx = Assert.ThrowsAsync<BadRequestException>(async delegate { await userService.AddUser(user); });

            Assert.That(actualEx.Message, Is.EqualTo("Something goes wrong!"));
        }
    }
}