using Microsoft.AspNetCore.Identity;

namespace RealWorldApp.DAL.Entities
{
    public class User : IdentityUser
    {
        public string Description { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
