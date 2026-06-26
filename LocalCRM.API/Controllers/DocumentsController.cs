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

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DocumentDto>>> GetDocuments()
    {
        return await _documentService.GetDocumentsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetDocument(int id)
    {
        var result = await _documentService.GetDocumentByIdAsync(id);
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> CreateDocument(CreateDocumentCommand command)
    {
        var result = await _documentService.CreateDocumentAsync(command);
        return CreatedAtAction(nameof(GetDocument), new { id = result.DocumentId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDocument(int id, UpdateDocumentCommand command)
    {
        if (id != command.DocumentId) return BadRequest();
        try { await _documentService.UpdateDocumentAsync(command); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        catch (Exception ex) when (ex.Message == "Concurrency conflict") { return Conflict(); }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        try { await _documentService.DeleteDocumentAsync(id); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        return NoContent();
    }
}
