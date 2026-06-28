using LocalCRM.Application.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    public NotesController(INoteService noteService) => _noteService = noteService;

    [HttpGet]
    public async Task<ActionResult<List<NoteDto>>> GetAll([FromQuery] bool includeDeleted = false) => await _noteService.GetAllAsync(includeDeleted);

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        var result = await _noteService.GetByIdAsync(id);
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create(CreateNoteCommand command)
    {
        var result = await _noteService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.NoteId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateNoteCommand command)
    {
        if (id != command.NoteId) return BadRequest();
        await _noteService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _noteService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _noteService.RestoreAsync(id);
        return NoContent();
    }
}
