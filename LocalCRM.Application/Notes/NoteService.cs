using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Notes;

public interface INoteService
{
    Task<List<NoteDto>> GetAllAsync(bool includeDeleted = false);
    Task<NoteDto?> GetByIdAsync(int id);
    Task<NoteDto> CreateAsync(CreateNoteCommand command);
    Task UpdateAsync(UpdateNoteCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}

public class NoteService : INoteService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public NoteService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<NoteDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Notes.AsQueryable();
        if (includeDeleted) query = query.IgnoreQueryFilters();
        return await query.ProjectTo<NoteDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<NoteDto?> GetByIdAsync(int id)
    {
        return await _context.Notes.IgnoreQueryFilters()
            .Where(n => n.NoteId == id)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<NoteDto> CreateAsync(CreateNoteCommand command)
    {
        var entity = _mapper.Map<Note>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        _context.Notes.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Note", EntityId = entity.NoteId, ActionType = "CREATE",
            PerformedBy = entity.CreatedBy, Notes = $"Created note: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
        return _mapper.Map<NoteDto>(entity);
    }

    public async Task UpdateAsync(UpdateNoteCommand command)
    {
        var entity = await _context.Notes.FindAsync(command.NoteId);
        if (entity == null) throw new Exception("Entity not found");
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt) throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.Subject != command.Subject) summary.Append($"Subject: {entity.Subject} -> {command.Subject}; ");
        if (entity.Body != command.Body) summary.Append($"Body changed; ");
        if (entity.Visibility != command.Visibility) summary.Append($"Visibility: {entity.Visibility} -> {command.Visibility}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Note", EntityId = entity.NoteId, ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy, Notes = summary.Length > 0 ? summary.ToString() : "No changes"
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Notes.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Note", EntityId = entity.NoteId, ActionType = "SOFT_DELETE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Deleted note: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Notes.IgnoreQueryFilters().FirstOrDefaultAsync(n => n.NoteId == id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Note", EntityId = entity.NoteId, ActionType = "RESTORE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Restored note: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
    }
}
