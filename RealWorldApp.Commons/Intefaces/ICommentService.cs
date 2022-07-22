using RealWorldApp.Commons.Models.CommentModel;
using System.Security.Claims;

namespace RealWorldApp.Commons.Intefaces
{
    public interface ICommentService
    {
        Task<CommentResponseModelContainer> AddComment(string slug, CreateCommentModelContainer createModel, ClaimsPrincipal claims);
        Task<CommentResponseModelContainerList> GetComments(string slug, ClaimsPrincipal claims);
        Task DeleteComment(int id);
    }
}
