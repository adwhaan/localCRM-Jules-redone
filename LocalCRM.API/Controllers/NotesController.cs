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

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<List<NoteDto>>> GetNotes()
    {
        return await _noteService.GetNotesAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteDto>> GetNote(int id)
    {
        var result = await _noteService.GetNoteByIdAsync(id);
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> CreateNote(CreateNoteCommand command)
    {
        var result = await _noteService.CreateNoteAsync(command);
        return CreatedAtAction(nameof(GetNote), new { id = result.NoteId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, UpdateNoteCommand command)
    {
        if (id != command.NoteId) return BadRequest();
        try { await _noteService.UpdateNoteAsync(command); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        catch (Exception ex) when (ex.Message == "Concurrency conflict") { return Conflict(); }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        try { await _noteService.DeleteNoteAsync(id); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        return NoContent();
    }
}
