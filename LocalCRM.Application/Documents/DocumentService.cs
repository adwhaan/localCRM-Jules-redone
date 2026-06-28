using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Documents;

public interface IDocumentService
{
    Task<List<DocumentDto>> GetAllAsync(bool includeDeleted = false);
    Task<DocumentDto?> GetByIdAsync(int id);
    Task<DocumentDto> CreateAsync(CreateDocumentCommand command);
    Task UpdateAsync(UpdateDocumentCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}

public class DocumentService : IDocumentService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DocumentService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<DocumentDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Documents.AsQueryable();
        if (includeDeleted) query = query.IgnoreQueryFilters();
        return await query.ProjectTo<DocumentDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<DocumentDto?> GetByIdAsync(int id)
    {
        return await _context.Documents.IgnoreQueryFilters()
            .Where(d => d.DocumentId == id)
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentDto> CreateAsync(CreateDocumentCommand command)
    {
        var entity = _mapper.Map<Document>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        _context.Documents.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Document", EntityId = entity.DocumentId, ActionType = "CREATE",
            PerformedBy = entity.CreatedBy, Notes = $"Created document: {entity.DocumentRef}"
        });
        await _context.SaveChangesAsync();
        return _mapper.Map<DocumentDto>(entity);
    }

    public async Task UpdateAsync(UpdateDocumentCommand command)
    {
        var entity = await _context.Documents.FindAsync(command.DocumentId);
        if (entity == null) throw new Exception("Entity not found");
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt) throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.DocumentRef != command.DocumentRef) summary.Append($"Ref: {entity.DocumentRef} -> {command.DocumentRef}; ");
        if (entity.Subject != command.Subject) summary.Append($"Subject: {entity.Subject} -> {command.Subject}; ");
        if (entity.DocumentType != command.DocumentType) summary.Append($"Type: {entity.DocumentType} -> {command.DocumentType}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Document", EntityId = entity.DocumentId, ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy, Notes = summary.Length > 0 ? summary.ToString() : "No changes"
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Documents.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Document", EntityId = entity.DocumentId, ActionType = "SOFT_DELETE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Deleted document: {entity.DocumentRef}"
        });
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Documents.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.DocumentId == id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Document", EntityId = entity.DocumentId, ActionType = "RESTORE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Restored document: {entity.DocumentRef}"
        });
        await _context.SaveChangesAsync();
    }
}
