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
    public TagsController(ITagService tagService) => _tagService = tagService;

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll() => await _tagService.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        var result = await _tagService.GetByIdAsync(id);
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagCommand command)
    {
        var result = await _tagService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.TagId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTagCommand command)
    {
        if (id != command.TagId) return BadRequest();
        await _tagService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tagService.DeleteAsync(id);
        return NoContent();
    }
}
