using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealWorldApp.DAL.Entities
{
    public class User : IdentityUser
    {
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string? URL { get; set; }
        public string? Bio { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
