﻿using AutoMapper;
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

        public async Task<string> GenerateJwt(string Email, string Password)
        {
            var user = await _userRepositorie.GetUserByEmail(Email);

            if (user is null)
            {
                throw new Exception("Invalid username or password");
            }

            if (_passwordHasher.Equals(Password == user.PasswordHash))
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    throw new Exception("Invalid username or password");
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
            var user = new User()
            {
                UserName = request.Username,
                Email = request.Email
            };

            var hashPassword = _passwordHasher.HashPassword(user, request.Password);

            user.PasswordHash = hashPassword;

            await _userRepositorie.AddUser(user);

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<List<ViewUserModel>> GetUsers()
        {
            var users = await _userRepositorie.GetUsers();
            return _mapper.Map<List<ViewUserModel>>(users);
        }

        public async Task<UserResponseContainer> GetUserByEmail(string Email)
        {
            var user = await _userRepositorie.GetUserByEmail(Email);
            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims)
        {
            var user = await _userRepositorie.GetUserById(claims.Identity.Name);
            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }

        public async Task<ProfileResponseContainer> GetProfile(string Username)
        {
            var user = await _userRepositorie.GetUserByUsername(Username);
            ProfileResponseContainer profileContainer = new ProfileResponseContainer() { Profile = _mapper.Map<ProfileResponse>(user) };

            return profileContainer;
        }

        public async Task<UserResponseContainer> UpdateUser(UserUpdateModelContainer request, ClaimsPrincipal claims, string token)
        {
            var user = await _userRepositorie.GetUserById(claims.Identity.Name);

            if (!String.IsNullOrEmpty(request.User.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.User.Password);
            }

            if (!String.IsNullOrEmpty(request.User.Image))
            {
                user.Image = request.User.Image;
            }

            user.UserName = request.User.UserName;
            user.Bio = request.User.Bio;
            user.Email = request.User.Email;
            user.Token = token;

            await _userRepositorie.SaveChangesAsync();

            UserResponseContainer userContainer = new UserResponseContainer() { User = _mapper.Map<UserResponse>(user) };

            return userContainer;
        }
    }
}