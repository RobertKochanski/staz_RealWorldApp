using Microsoft.AspNetCore.Mvc;
using RealWorldApp.Commons.Intefaces;

namespace RealWebAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _tagService.GetTags());
        }
    }
}
