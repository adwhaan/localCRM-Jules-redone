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

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> GetContacts()
    {
        return await _contactService.GetContactsAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetContact(int id)
    {
        var contact = await _contactService.GetContactByIdAsync(id);
        if (contact == null) return NotFound();
        return contact;
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> CreateContact(CreateContactCommand command)
    {
        var contact = await _contactService.CreateContactAsync(command);
        return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, UpdateContactCommand command)
    {
        if (id != command.ContactId) return BadRequest();
        try { await _contactService.UpdateContactAsync(command); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        catch (Exception ex) when (ex.Message == "Concurrency conflict") { return Conflict(); }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        try { await _contactService.DeleteContactAsync(id); }
        catch (Exception ex) when (ex.Message == "Entity not found") { return NotFound(); }
        return NoContent();
    }
}
