using LocalCRM.Application.Interactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public InteractionsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<InteractionDto>>> GetInteractions()
    {
        return await _interactionService.GetInteractionsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InteractionDto>> GetInteraction(int id)
    {
        var result = await _interactionService.GetInteractionByIdAsync(id);
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<InteractionDto>> CreateInteraction(CreateInteractionCommand command)
    {
        var result = await _interactionService.CreateInteractionAsync(command);
        return CreatedAtAction(nameof(GetInteraction), new { id = result.InteractionId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInteraction(int id, UpdateInteractionCommand command)
    {
        if (id != command.InteractionId) return BadRequest();
        try { await _interactionService.UpdateInteractionAsync(command); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        catch (Exception ex) when (ex.Message == "Concurrency conflict") { return Conflict(); }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInteraction(int id)
    {
        try { await _interactionService.DeleteInteractionAsync(id); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        return NoContent();
    }
}
