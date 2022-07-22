using Microsoft.AspNetCore.Identity;

namespace RealWorldApp.Commons.Entities
{
    public class User : IdentityUser
    {
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string? Token { get; set; }

        public ICollection<Article> Articles { get; set; } = new List<Article>();
        public ICollection<Article> FavoriteArticles { get; set; } = new List<Article>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<User> FollowedUsers { get; set; } = new List<User>();
    }
}
