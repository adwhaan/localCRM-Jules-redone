using LocalCRM.Application.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService) => _contactService = contactService;

    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> GetAll([FromQuery] bool includeDeleted = false) => await _contactService.GetAllAsync(includeDeleted);

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetById(int id)
    {
        var contact = await _contactService.GetByIdAsync(id);
        return contact == null ? NotFound() : contact;
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(CreateContactCommand command)
    {
        var contact = await _contactService.CreateAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = contact.ContactId }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateContactCommand command)
    {
        if (id != command.ContactId) return BadRequest();
        await _contactService.UpdateAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contactService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        await _contactService.RestoreAsync(id);
        return NoContent();
    }
}
