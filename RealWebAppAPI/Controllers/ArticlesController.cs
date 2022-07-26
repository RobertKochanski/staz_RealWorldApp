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
        public async Task<IActionResult> GetArticles([FromQuery]string? author, [FromQuery]string? favorited, [FromQuery] string? tag, [FromQuery]int limit, [FromQuery]int offset)
        {
            return Ok(await _articleService.GetArticles(author, favorited, tag, limit, offset, User));
        }

        [AllowAnonymous]
        [HttpGet("articles/{slug}")]
        public async Task<IActionResult> GetArticleBySlug([FromRoute]string slug)
        {
            return Ok(await _articleService.GetArticleBySlug(slug, User));
        }

        [HttpGet("articles/feed")]
        public async Task<IActionResult> GetArticleFeed([FromQuery] int limit, [FromQuery] int offset)
        {
            return Ok(await _articleService.GetArticleFeed(limit, offset, User));
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

        [HttpPost("articles/{slug}/favorite")]
        public async Task<IActionResult> AddFavorite([FromRoute] string slug)
        {
            return Ok(await _articleService.AddFavorite(slug, User));
        }

        [HttpDelete("articles/{slug}/favorite")]
        public async Task<IActionResult> UnFavorite([FromRoute] string slug)
        {
            return Ok(await _articleService.UnFavorite(slug, User));
        }
    }
}
