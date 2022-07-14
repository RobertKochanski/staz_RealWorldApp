namespace RealWorldApp.Commons.Entities
{
    public class Article : BaseEntitie
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ArticleText { get; set; }
        public DateTime PublicDate { get; set; }
        public int FaroritesCount { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
