using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.BAL.Models
{
    public class ProfileResponseContainer
    {
        public ProfileResponse Profile { get; set; }
    }

    public class ProfileResponse
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public bool following { get; set; } = false;
    }
}
