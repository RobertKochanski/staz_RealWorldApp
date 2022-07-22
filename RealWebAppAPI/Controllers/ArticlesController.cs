using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldApp.Commons.Intefaces;
using RealWorldApp.Commons.Models.ArticleModel;

namespace RealWebAppAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }


        [HttpPost("articles")]
        public async Task<IActionResult> AddArticle(CreateUpdateArticleModelContainer article)
        {
            return Ok(await _articleService.AddArticle(article, User));
        }

        [AllowAnonymous]
        [HttpGet("articles")]
        public async Task<IActionResult> GetArticles([FromQuery]string? author, [FromQuery]string? favorited, [FromQuery]int limit, [FromQuery]int offset)
        {
            return Ok(await _articleService.GetArticles(author, favorited, limit, offset));
        }

        [AllowAnonymous]
        [HttpGet("articles/{slug}")]
        public async Task<IActionResult> GetArticleBySlug([FromRoute]string slug)
        {
            return Ok(await _articleService.GetArticleBySlug(slug, User));
        }

        [HttpGet("articles/feed")]
        public async Task<IActionResult> GetArticleFeed([FromRoute] string feed, [FromQuery] int limit, [FromQuery] int offset)
        {
            return Ok();
        }

        [HttpDelete("articles/{slug}")]
        public async Task<IActionResult> DeleteArticle([FromRoute]string slug)
        {
            await _articleService.DeleteArticle(slug);

            return Ok();
        }

        [HttpPut("articles/{slug}")]
        public async Task<IActionResult> UpdateArticle([FromRoute] string slug, CreateUpdateArticleModelContainer updateModel)
        {
            return Ok(await _articleService.UpdateArticle(slug, updateModel));
        }
    }
}
