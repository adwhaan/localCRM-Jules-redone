using LocalCRM.Application.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    public DocumentsController(IDocumentService documentService) => _documentService = documentService;

    [HttpGet]
    public async Task<ActionResult<List<DocumentDto>>> GetAll([FromQuery] bool includeDeleted = false) => await _documentService.GetAllAsync(includeDeleted);

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(int id)
    {
        var result = await _documentService.GetByIdAsync(id);
        return result == null ? NotFound() : result;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> Create(CreateDocumentCommand command)
    {
        var result = await _documentService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = result.DocumentId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDocumentCommand command)
    {
        if (id != command.DocumentId) return BadRequest();
        await _documentService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _documentService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _documentService.RestoreAsync(id);
        return NoContent();
    }
}
