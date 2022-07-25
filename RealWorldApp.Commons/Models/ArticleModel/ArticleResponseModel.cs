namespace RealWorldApp.Commons.Models.ArticleModel
{
    public class ArticleResponseModelContainer
    {
        public ArticleResponseModel Article { get; set; }
    }

    public class ArticleResponseModelContainerList
    {
        public List<ArticleResponseModel> Articles { get; set; }
        public int ArticlesCount { get; set; }
    }

    public class ArticleResponseModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public List<string> TagList { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool Favorited { get; set; }
        public int FavoritesCount { get; set; }
        public UserArticleResponseModel Author { get; set; }
    }
}
