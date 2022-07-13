using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.BAL.Models
{
    public class UserResponseContainer
    {
        public UserResponse User { get; set; }
    }

    public class UserResponse
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
