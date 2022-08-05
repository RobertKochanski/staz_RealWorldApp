using MediatR;
using Microsoft.AspNetCore.Identity;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.CQRS.Users.Commends
{
    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserResponseContainer>
    {
        private readonly IUserService _userService;

        public AuthenticateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserResponseContainer> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByEmail(request.User.Email);
            var token = await _userService.GenerateJwt(request.User.Email, request.User.Password);

            user.User.Token = token;

            return user;
        }
    }
}
