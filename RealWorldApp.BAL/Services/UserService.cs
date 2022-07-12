using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RealWorldApp.BAL.Models;
using RealWorldApp.BAL.Services.Intefaces;
using RealWorldApp.DAL.Entities;
using RealWorldApp.DAL.Repositories.Interfaces;
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
        private readonly AuthenticationSettings _authenticationSettings;

        public UserService(IUserRepositorie userRepositorie, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _userRepositorie = userRepositorie;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public async Task<string> GenerateJwt(UserLogin model)
        {
            var user = await _userRepositorie.GetUserByEmail(model.Email);

            if (user is null)
            {
                throw new Exception("Invalid username or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.UserName}"),
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

        public async Task AddUser(UserRegister request)
        {
            var user = new User()
            {
                UserName = request.Username,
                Email = request.Email
            };

            var hashPassword = _passwordHasher.HashPassword(user, request.Password);

            user.PasswordHash = hashPassword;

            await _userRepositorie.AddUser(user);
        }

        public async Task<List<ViewUserModel>> GetUsers()
        {
            var users = await _userRepositorie.GetUsers();
            return _mapper.Map<List<ViewUserModel>>(users);
        }

        public async Task<ViewUserModel> GetUserByEmail(string Email)
        {
            var user = await _userRepositorie.GetUserByEmail(Email);
            return _mapper.Map<ViewUserModel>(user);
        }

        public async Task<ViewUserModel> GetUserById(string Id)
        {
            var user = await _userRepositorie.GetUserById(Id);
            return _mapper.Map<ViewUserModel>(user);
        }

        public async Task UpdateUser(string id, UserUpdateModel request)
        {
            var user = await _userRepositorie.GetUserById(id);

            user.UserName = request.UserName;
            user.Bio = request.Bio;
            user.URL = request.URL;
            user.Email = request.Email;
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepositorie.SaveChangesAsync();
            
        }
    }
}
