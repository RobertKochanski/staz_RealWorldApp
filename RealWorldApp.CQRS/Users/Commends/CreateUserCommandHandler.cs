using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.UserModel;

namespace RealWorldApp.CQRS.Users.Commends
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseContainer>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IMapper mapper, UserManager<User> userManager, IUserService userService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _userService = userService;
        }

        public async Task<UserResponseContainer> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            User user = new User()
            {
                UserName = request.User.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = request.User.Email,
            };

            var result = await _userManager.CreateAsync(user, request.User.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(string.Join(" ", result.Errors.Select(x => x.Description)));
            }

            var userResponse = _mapper.Map<UserResponse>(user);
            var token = await _userService.GenerateJwt(request.User.Email, request.User.Password);
            userResponse.Token = token;

            var response = new UserResponseContainer { User = userResponse };

            return response;
        }
    }
}
