using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealWorldApp.DAL.Entities
{
    public class Users : BaseEntitie
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
