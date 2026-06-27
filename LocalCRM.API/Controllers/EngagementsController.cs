using LocalCRM.Application.Engagements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EngagementsController : ControllerBase
{
    private readonly IEngagementService _engagementService;
    public EngagementsController(IEngagementService engagementService) => _engagementService = engagementService;

    [HttpGet]
    public async Task<ActionResult<List<EngagementDto>>> GetAll([FromQuery] bool includeDeleted = false) => await _engagementService.GetAllAsync(includeDeleted);

    [HttpGet("{id}")]
    public async Task<ActionResult<EngagementDto>> GetById(int id)
    {
        var result = await _engagementService.GetByIdAsync(id);
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<EngagementDto>> Create(CreateEngagementCommand command)
    {
        var result = await _engagementService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.EngagementId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEngagementCommand command)
    {
        if (id != command.EngagementId) return BadRequest();
        await _engagementService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _engagementService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _engagementService.RestoreAsync(id);
        return NoContent();
    }
}
