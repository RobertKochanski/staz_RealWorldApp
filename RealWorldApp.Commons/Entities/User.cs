using Microsoft.AspNetCore.Identity;

namespace RealWorldApp.Commons.Entities
{
    public class User : IdentityUser
    {
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string? Token { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
