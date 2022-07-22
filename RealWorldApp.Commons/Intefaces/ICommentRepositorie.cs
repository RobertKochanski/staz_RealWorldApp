using RealWorldApp.Commons.Entities;

namespace RealWorldApp.Commons.Intefaces
{
    public interface ICommentRepositorie
    {
        Task AddComment(Comment comment);
        Task<List<Comment>> GetCommentsForArticle(string slug);
        Task<Comment> GetCommentById(int id);
        void DeleteComment(Comment comment);
    }
}
