using Microsoft.AspNetCore.Identity;

namespace RealWorldApp.DAL.Entities
{
    public class User : IdentityUser
    {
        public string? URL { get; set; }
        public string? Bio { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
