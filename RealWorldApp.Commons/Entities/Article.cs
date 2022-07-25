namespace RealWorldApp.Commons.Entities
{
    public class Article : BaseEntitie
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public bool Favorited { get; set; }
        public int FavoritesCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public User Author { get; set; }

        public List<Tag> TagList { get; set; } = new List<Tag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
