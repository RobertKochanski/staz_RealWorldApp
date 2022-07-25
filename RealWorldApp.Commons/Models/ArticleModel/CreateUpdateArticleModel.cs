namespace RealWorldApp.Commons.Models.ArticleModel
{
    public class CreateUpdateArticleModelContainer
    {
        public CreateUpdateArticleModel Article { get; set; }
    }

    public class CreateUpdateArticleModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public List<string> TagList { get; set; } = new List<string>();
    }
}
