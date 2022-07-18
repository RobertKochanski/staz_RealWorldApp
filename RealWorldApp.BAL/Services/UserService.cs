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
        private readonly IUserRepositorie _userRepositorie;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger _logger;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepositorie userRepositorie, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, UserManager<User> userManager, ILogger logger)
        {
            _userRepositorie = userRepositorie;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GenerateJwt(string Email, string Password)
        {
            var user = await _userRepositorie.GetUserByEmail(Email);
            //var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            if (!Password.Equals(user.PasswordHash))
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);

                if (result == PasswordVerificationResult.Failed)
                {
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
                Email = request.Email,
                UserName = request.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Can't create account with this data!");
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
            var user = await _userRepositorie.GetUserByEmail(Email);

            if (user == null)
            {
                throw new BadRequestException("Something goes wrong!");
            }

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user == null)
            {
                throw new BadRequestException("Something goes wrong!");
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
                throw new BadRequestException("Something goes wrong!");
            }

            ProfileResponseContainer profileContainer = new ProfileResponseContainer() { Profile = _mapper.Map<ProfileResponse>(user) };

            return profileContainer;
        }

        public async Task<UserResponseContainer> UpdateUser(UserUpdateModelContainer request, ClaimsPrincipal claims)
        {
            var user = await _userManager.FindByIdAsync(claims.Identity.Name);

            if (user == null)
            {
                throw new BadRequestException("Something goes wrong!");
            }

            if (!string.IsNullOrEmpty(request.User.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);
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
                throw new BadRequestException("Can't update account with this data!");
            }

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }
    }
}
