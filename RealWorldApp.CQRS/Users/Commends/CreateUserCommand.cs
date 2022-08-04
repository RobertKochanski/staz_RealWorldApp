using MediatR;
using RealWorldApp.Commons.Models.UserModel;

namespace RealWorldApp.CQRS.Users.Commends
{
    public class CreateUserCommand : IRequest<UserResponseContainer>
    {
        public UserRegister user { get; set; }
    }
}
