using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealWorldApp.BAL;
using RealWorldApp.BAL.Services;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models;
using System.Security.Claims;

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
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success); 

            var userService = new UserService(null, mockMapper.Object, null,null, userManager.Object);

            //Act
            var actual = await userService.AddUser(user);

            //Assert
            Assert.IsNotNull(actual);
            mockMapper.Verify(x => x.Map<UserResponse>(It.IsAny<User>()), Times.Once());
            Assert.That(actual.User.Username, Is.EqualTo(expected.User.Username));
        }

        [Category("RegisterUser")]
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
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>().And.Message.EqualTo("Can't create account with this data!"), async delegate { await userService.AddUser(user); });

            var actualEx = Assert.ThrowsAsync<BadRequestException>(async delegate { await userService.AddUser(user); });

            Assert.That(actualEx.Message, Is.EqualTo("Can't create account with this data!"));
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

            Mock<IUserRepositorie> mockRepositorie = new Mock<IUserRepositorie>();
            mockRepositorie.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(response);

            var userService = new UserService(mockRepositorie.Object, mockMapper.Object, null, null, null);

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
        public async Task GetUserByEmail_WithUncorrectData_ThrowBadRequestException()
        {
            //Arrange
            User user = null;

            Mock<IUserRepositorie> mockRepositorie = new Mock<IUserRepositorie>();
            mockRepositorie.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(mockRepositorie.Object, null, null, null, null);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Something goes wrong!"), async delegate { await userService.GetUserByEmail("tezter@tester.com"); });
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

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IMapper> mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<ProfileResponse>(It.IsAny<User>())).Returns(response);

            var userService = new UserService(null, mockMapper.Object, null, null, mockUserManager.Object);

            //Act
            var actual = await userService.GetProfile("tester");

            //Assert
            Assert.IsNotNull(actual);
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()));
            Assert.That(actual, Is.InstanceOf(expected.GetType()));
        }

        [Category("GetProfile")]
        [Test]
        public async Task GetMyProfile_WithUncorrectData_ThrowBadRequestException()
        {
            //Arrange
            User user = null;

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(null, null, null, null, mockUserManager.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Something goes wrong!"), async delegate { await userService.GetProfile("Username"); });
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

            Mock<IUserRepositorie> mockRepositorie = new Mock<IUserRepositorie>();
            mockRepositorie.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

            Mock<IPasswordHasher<User>> mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, userLogin.Password)).Returns(PasswordVerificationResult.Success);            

            var userService = new UserService(mockRepositorie.Object, null, mockPasswordHasher.Object, authenticationSettings, null);

            //Act
            var actual = await userService.GenerateJwt(userLogin.Email, userLogin.Password);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Category("GenerateJwt")]
        [Test]
        public async Task GenerateToken_WithUncorrectUsername_ThrowBadRequestException()
        {
            //Arrange
            UserLogin userLogin = new UserLogin
            {
                Email = "test@test.com",
                Password = "zaq1@WSX"
            };

            User user = null;

            Mock<IUserRepositorie> mockRepositorie = new Mock<IUserRepositorie>();
            mockRepositorie.Setup(x => x.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(mockRepositorie.Object, null, null, null, null);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Invalid username or password"), async delegate { await userService.GenerateJwt("tester@tester.com", "haslo"); });
        }

        [Category("GenerateJwt")]
        [Test]
        public async Task GenerateToken_WithUncorrectPassword_ThrowBadRequestException()
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

            Mock<IPasswordHasher<User>> mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, userLogin.Password)).Returns(PasswordVerificationResult.Failed);

            var userService = new UserService(mockRepositorie.Object, null, mockPasswordHasher.Object, null, null);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Invalid username or password"), async delegate { await userService.GenerateJwt("tester@tester.com", "haslo"); });
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

            var userService = new UserService(null, mockMapper.Object, mockPasswordHasher.Object, null, mockUserManager.Object);

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

            var userService = new UserService(null, null, null, null, mockUserManager.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Something goes wrong!"), async delegate { await userService.UpdateUser(updateModel, claimsPrincipal); });
        }

        [Category("UpdateUser")]
        [Test]
        public async Task UpdateUser_WithUncorrectData_ThrowBadRequestException()
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

            Mock<UserManager<User>> mockUserManager = GetMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(userToChange);
            mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            Mock<IPasswordHasher<User>> mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(userToChange.PasswordHash);

            var userService = new UserService(null, null, mockPasswordHasher.Object, null, mockUserManager.Object);

            //Act

            //Assert
            Assert.ThrowsAsync(Is.TypeOf<BadRequestException>()
                .And.Message.EqualTo("Can't update account with this data!"), async delegate { await userService.UpdateUser(updateModel, claimsPrincipal); });
        }
    }
}