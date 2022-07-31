using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RealWorldApp.Commons.Entities;
using RealWorldApp.Commons.Exceptions;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.CommentModel;
using System.Security.Claims;

namespace RealWorldApp.BAL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepositorie _commentRepositorie;
        private readonly IArticleRepositorie _articleRepositorie;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepositorie commentRepositorie, UserManager<User> userManager, IArticleRepositorie articleRepositorie, IMapper mapper)
        {
            _commentRepositorie = commentRepositorie;
            _userManager = userManager;
            _articleRepositorie = articleRepositorie;
            _mapper = mapper;
        }

        public async Task<CommentResponseModelContainer> AddComment(string slug, CreateCommentModelContainer createModel, ClaimsPrincipal claims)
        {
            User user = await _userManager.FindByIdAsync(claims.Identity.Name);
            Article article = await _articleRepositorie.GetArticleBySlug(slug);

            if (article == null)
            {
                throw new BadRequestException("Can't find article!");
            }

            Comment comment = new Comment
            {
                Body = createModel.Comment.Body,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Author = user,
                Article = article,
            };

            await _commentRepositorie.AddComment(comment);

            var commentResponse = _mapper.Map<CommentResponseModel>(comment);

            CommentResponseModelContainer responseModel = new CommentResponseModelContainer() { Comment = commentResponse};

            return responseModel;
        }

        public async Task<CommentResponseModelContainerList> GetComments(string slug, ClaimsPrincipal claims)
        {
            var commentsList = await _commentRepositorie.GetCommentsForArticle(slug);

            CommentResponseModelContainerList response = new CommentResponseModelContainerList() { Comments = _mapper.Map<List<CommentResponseModel>>(commentsList) };

            return response;
        }

        public async Task DeleteComment(int id)
        {
            var comment = await _commentRepositorie.GetCommentById(id);

            _commentRepositorie.DeleteComment(comment);
        }
    }
}
