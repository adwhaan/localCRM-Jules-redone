using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Contacts;

public interface IContactService
{
    Task<List<ContactDto>> GetAllAsync(bool includeDeleted = false);
    Task<ContactDto?> GetByIdAsync(int id);
    Task<ContactDto> CreateAsync(CreateContactCommand command);
    Task UpdateAsync(UpdateContactCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
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

    public async Task<List<ContactDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Contacts.AsQueryable();
        if (includeDeleted) query = query.IgnoreQueryFilters();
        return await query.ProjectTo<ContactDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<ContactDto?> GetByIdAsync(int id)
    {
        return await _context.Contacts.IgnoreQueryFilters()
            .Where(c => c.ContactId == id)
            .ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<ContactDto> CreateAsync(CreateContactCommand command)
    {
        var entity = _mapper.Map<Contact>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        _context.Contacts.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Contact", EntityId = entity.ContactId, ActionType = "CREATE",
            PerformedBy = entity.CreatedBy, Notes = $"Created contact: {entity.FirstName} {entity.LastName}"
        });
        await _context.SaveChangesAsync();
        return _mapper.Map<ContactDto>(entity);
    }

    public async Task UpdateAsync(UpdateContactCommand command)
    {
        var entity = await _context.Contacts.FindAsync(command.ContactId);
        if (entity == null) throw new Exception("Entity not found");
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt) throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.FirstName != command.FirstName) summary.Append($"FirstName: {entity.FirstName} -> {command.FirstName}; ");
        if (entity.LastName != command.LastName) summary.Append($"LastName: {entity.LastName} -> {command.LastName}; ");
        if (entity.Email != command.Email) summary.Append($"Email: {entity.Email} -> {command.Email}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Contact", EntityId = entity.ContactId, ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy, Notes = summary.Length > 0 ? summary.ToString() : "No changes"
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Contacts.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Contact", EntityId = entity.ContactId, ActionType = "SOFT_DELETE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Deleted contact: {entity.FirstName}"
        });
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Contacts.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.ContactId == id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Contact", EntityId = entity.ContactId, ActionType = "RESTORE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Restored contact: {entity.FirstName}"
        });
        await _context.SaveChangesAsync();
    }
}
