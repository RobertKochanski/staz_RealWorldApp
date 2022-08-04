using MediatR;
using RealWorldApp.Commons.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.CQRS.Users.Commends
{
    public class AuthenticateUserCommand : IRequest<UserResponseContainer>
    {
        public UserLogin User { get; set; }
    }
}
