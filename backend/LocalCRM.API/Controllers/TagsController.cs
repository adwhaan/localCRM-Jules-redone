using LocalCRM.Application.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        return await _tagService.GetTagsAsync();
    }

    [HttpGet("group/{group}")]
    public async Task<ActionResult<List<TagDto>>> GetTagsByGroup(string group)
    {
        return await _tagService.GetTagsByGroupAsync(group);
    }
}
