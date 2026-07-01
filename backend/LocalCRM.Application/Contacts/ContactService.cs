using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Contacts;

public interface IContactService
{
    Task<List<ContactDto>> GetContactsAsync();
    Task<ContactDto?> GetContactByIdAsync(int id);
    Task<ContactDto> CreateContactAsync(CreateContactCommand command);
    Task UpdateContactAsync(UpdateContactCommand command);
    Task DeleteContactAsync(int id);
}

public class ContactService : IContactService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ContactService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<ContactDto>> GetContactsAsync()
    {
        return await _context.Contacts
            .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ContactDto?> GetContactByIdAsync(int id)
    {
        return await _context.Contacts
            .Where(c => c.ContactId == id)
            .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<ContactDto> CreateContactAsync(CreateContactCommand command)
    {
        var entity = _mapper.Map<Contact>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        entity.CreatedAt = DateTime.UtcNow;

        _context.Contacts.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "contacts",
            EntityId = entity.ContactId,
            ActionType = "CREATE",
            PerformedBy = entity.CreatedBy,
            Notes = $"Created contact {entity.FirstName} {entity.LastName}"
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<ContactDto>(entity);
    }

    public async Task UpdateContactAsync(UpdateContactCommand command)
    {
        var entity = await _context.Contacts.FindAsync(command.ContactId);

        if (entity == null) throw new Exception("Entity not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "contacts",
            EntityId = entity.ContactId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated contact {entity.FirstName} {entity.LastName}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteContactAsync(int id)
    {
        var entity = await _context.Contacts.FindAsync(id);

        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "contacts",
            EntityId = entity.ContactId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted contact {entity.FirstName} {entity.LastName}"
        });

        await _context.SaveChangesAsync();
    }
}
