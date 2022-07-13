using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealWorldApp.DAL.Entities
{
    public class User : IdentityUser
    {
        public string Email { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string? Token { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
