namespace RealWorldApp.Commons.Entities
{
    public class Comment : BaseEntitie
    {
        public string text { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
    }
}
