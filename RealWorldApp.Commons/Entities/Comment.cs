namespace RealWorldApp.Commons.Entities
{
    public class Comment : BaseEntitie
    {
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User Author { get; set; }
        public Article Article { get; set; }
    }
}
