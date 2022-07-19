using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealWorldApp.BAL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly UserManager<User> _userManager;

        public UserService(IMapper mapper, AuthenticationSettings authenticationSettings, UserManager<User> userManager, ILogger<UserService> logger)
        {
            _mapper = mapper;
            _authenticationSettings = authenticationSettings;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GenerateJwt(string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                _logger.LogError("Invalid username or password");
                throw new BadRequestException("Invalid username or password");
            }

            if (!Password.Equals(user.PasswordHash))
            {
                var result = await _userManager.CheckPasswordAsync(user, Password);

                if (!result)
                {
                    _logger.LogError("Invalid username or password");
                    throw new BadRequestException("Invalid username or password");
                }
            }
            
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, $"{user.Email}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public async Task<UserResponseContainer> AddUser(UserRegister request)
        {
            User user = new User()
            {
                UserName = request.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = request.Email,
            };

            var emailResult = await _userManager.FindByEmailAsync(request.Email);

            if (emailResult != null)
            {
                _logger.LogError("User with this email already exist");
                throw new ArgumentException("User with this email already exist");
            }
            
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                _logger.LogError(string.Join(", ", result.Errors.Select(x => x.Description)));
                throw new BadRequestException(string.Join(" ", result.Errors.Select(x => x.Description)));
            }

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<List<ViewUserModel>> GetUsers()
        {
            var usersList = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<ViewUserModel>>(usersList);
        }

        public async Task<UserResponseContainer> GetUserByEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                _logger.LogError("Can't find user");
                throw new BadRequestException("Can't find user");
            }

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user == null)
            {
                _logger.LogError("Can't find your info");
                throw new BadRequestException("Can't find your info");
            }

            string token = await GenerateJwt(user.Email, user.PasswordHash);

            user.Token = token;

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<ProfileResponseContainer> GetProfile(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);

            if (user == null)
            {
                _logger.LogError("Can't get your profile");
                throw new BadRequestException("Can't get your profile");
            }

            ProfileResponseContainer profileContainer = new ProfileResponseContainer() { Profile = _mapper.Map<ProfileResponse>(user) };

            return profileContainer;
        }

        public async Task<UserResponseContainer> UpdateUser(UserUpdateModelContainer request, ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user == null)
            {
                _logger.LogError("Can't find user");
                throw new BadRequestException("Can't find user");
            }

            if (!string.IsNullOrEmpty(request.User.Password))
            {
                var passwordValidator = new PasswordValidator<User>();
                var passwordResult = await passwordValidator.ValidateAsync(_userManager, user, request.User.Password);

                if (passwordResult.Succeeded)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.User.Password);
                }
                else
                {
                    _logger.LogError(string.Join(", ", passwordResult.Errors.Select(x => x.Description)));
                    throw new BadRequestException(string.Join(" ", passwordResult.Errors.Select(x => x.Description)));
                }
            }

            if (!string.IsNullOrEmpty(request.User.UserName))
            {
                user.UserName = request.User.UserName;
            }

            if (!string.IsNullOrEmpty(request.User.Email))
            {
                user.Email = request.User.Email;
            }
            
            user.Bio = request.User.Bio;
            user.Image = request.User.Image;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError(string.Join(", ", result.Errors.Select(x => x.Description)));
                throw new BadRequestException(string.Join(" ", result.Errors.Select(x => x.Description)));
            }

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }
    }
}
