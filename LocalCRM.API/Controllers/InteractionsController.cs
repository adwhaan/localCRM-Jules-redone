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

    public InteractionsController(IInteractionService interactionService) => _interactionService = interactionService;

    [HttpGet]
    public async Task<ActionResult<List<InteractionDto>>> GetAll([FromQuery] bool includeDeleted = false) => await _interactionService.GetAllAsync(includeDeleted);

    [HttpGet("{id}")]
    public async Task<ActionResult<InteractionDto>> GetById(int id)
    {
        var result = await _interactionService.GetByIdAsync(id);
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<InteractionDto>> Create(CreateInteractionCommand command)
    {
        var result = await _interactionService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.InteractionId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateInteractionCommand command)
    {
        if (id != command.InteractionId) return BadRequest();
        await _interactionService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _interactionService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _interactionService.RestoreAsync(id);
        return NoContent();
    }
}
