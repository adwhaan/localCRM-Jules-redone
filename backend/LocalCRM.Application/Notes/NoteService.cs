using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Notes;

public interface INoteService
{
    Task<List<NoteDto>> GetNotesAsync();
    Task<NoteDto?> GetNoteByIdAsync(int id);
    Task<NoteDto> CreateNoteAsync(CreateNoteCommand command);
    Task UpdateNoteAsync(UpdateNoteCommand command);
    Task DeleteNoteAsync(int id);
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

    public async Task<List<NoteDto>> GetNotesAsync()
    {
        return await _context.Notes
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<NoteDto?> GetNoteByIdAsync(int id)
    {
        return await _context.Notes
            .Where(n => n.NoteId == id)
            .ProjectTo<NoteDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<NoteDto> CreateNoteAsync(CreateNoteCommand command)
    {
        using var transaction = await ((DbContext)_context).Database.BeginTransactionAsync();
        try
        {
            var entity = _mapper.Map<Note>(command);
            entity.CreatedBy = _currentUserService.Username ?? "system";
            entity.CreatedAt = DateTime.UtcNow;

            _context.Notes.Add(entity);
            await _context.SaveChangesAsync();

            // Linkage
            if (command.CompanyId.HasValue)
                _context.CompanyNoteLinks.Add(new CompanyNoteLink { CompanyId = command.CompanyId.Value, NoteId = entity.NoteId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });
            if (command.ContactId.HasValue)
                _context.ContactNoteLinks.Add(new ContactNoteLink { ContactId = command.ContactId.Value, NoteId = entity.NoteId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });
            if (command.InteractionId.HasValue)
                _context.InteractionNoteLinks.Add(new InteractionNoteLink { InteractionId = command.InteractionId.Value, NoteId = entity.NoteId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });
            if (command.EngagementId.HasValue)
                _context.EngagementNoteLinks.Add(new EngagementNoteLink { EngagementId = command.EngagementId.Value, NoteId = entity.NoteId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "notes",
                EntityId = entity.NoteId,
                ActionType = "CREATE",
                PerformedBy = entity.CreatedBy,
                Notes = $"Created note: {entity.Subject}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<NoteDto>(entity);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateNoteAsync(UpdateNoteCommand command)
    {
        var entity = await _context.Notes.FindAsync(command.NoteId);

        if (entity == null) throw new Exception("Entity not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "notes",
            EntityId = entity.NoteId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated note: {entity.Subject}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteNoteAsync(int id)
    {
        var entity = await _context.Notes.FindAsync(id);

        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "notes",
            EntityId = entity.NoteId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted note: {entity.Subject}"
        });

        await _context.SaveChangesAsync();
    }
}
