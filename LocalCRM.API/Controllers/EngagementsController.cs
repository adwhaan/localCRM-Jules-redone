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

    public EngagementsController(IEngagementService engagementService)
    {
        _engagementService = engagementService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EngagementDto>>> GetEngagements()
    {
        return await _engagementService.GetEngagementsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EngagementDto>> GetEngagement(int id)
    {
        var result = await _engagementService.GetEngagementByIdAsync(id);
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<EngagementDto>> CreateEngagement(CreateEngagementCommand command)
    {
        var result = await _engagementService.CreateEngagementAsync(command);
        return CreatedAtAction(nameof(GetEngagement), new { id = result.EngagementId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEngagement(int id, UpdateEngagementCommand command)
    {
        if (id != command.EngagementId) return BadRequest();
        try { await _engagementService.UpdateEngagementAsync(command); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        catch (Exception ex) when (ex.Message == "Concurrency conflict") { return Conflict(); }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEngagement(int id)
    {
        try { await _engagementService.DeleteEngagementAsync(id); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        return NoContent();
    }
}
