using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.CommentModel;

namespace RealWebAppAPI.Controllers
{
    [Authorize]
    [Route("api/articles/{slug}/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentsService;

        public CommentsController(ICommentService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromRoute]string slug, CreateCommentModelContainer createModel)
        {
            return Ok(await _commentsService.AddComment(slug, createModel, User));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetComments([FromRoute]string slug)
        {
            return Ok(await _commentsService.GetComments(slug, User));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute]string slug, [FromRoute]int id)
        {
            await _commentsService.DeleteComment(id);

            return Ok();
        }
    }
}
