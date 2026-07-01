using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Documents;

public interface IDocumentService
{
    Task<List<DocumentDto>> GetDocumentsAsync();
    Task<DocumentDto?> GetDocumentByIdAsync(int id);
    Task<DocumentDto> CreateDocumentAsync(CreateDocumentCommand command);
    Task UpdateDocumentAsync(UpdateDocumentCommand command);
    Task DeleteDocumentAsync(int id);
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

    public async Task<List<DocumentDto>> GetDocumentsAsync()
    {
        return await _context.Documents
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(int id)
    {
        return await _context.Documents
            .Where(d => d.DocumentId == id)
            .ProjectTo<DocumentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentDto> CreateDocumentAsync(CreateDocumentCommand command)
    {
        using var transaction = await ((DbContext)_context).Database.BeginTransactionAsync();
        try
        {
            var entity = _mapper.Map<Document>(command);
            entity.CreatedBy = _currentUserService.Username ?? "system";
            entity.CreatedAt = DateTime.UtcNow;

            _context.Documents.Add(entity);
            await _context.SaveChangesAsync();

            // Linkage
            if (command.CompanyId.HasValue)
                _context.CompanyDocumentLinks.Add(new CompanyDocumentLink { CompanyId = command.CompanyId.Value, DocumentId = entity.DocumentId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });
            if (command.InteractionId.HasValue)
                _context.InteractionDocumentLinks.Add(new InteractionDocumentLink { InteractionId = command.InteractionId.Value, DocumentId = entity.DocumentId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });
            if (command.EngagementId.HasValue)
                _context.EngagementDocumentLinks.Add(new EngagementDocumentLink { EngagementId = command.EngagementId.Value, DocumentId = entity.DocumentId, CreatedBy = entity.CreatedBy, CreatedAt = DateTime.UtcNow });

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "documents",
                EntityId = entity.DocumentId,
                ActionType = "CREATE",
                PerformedBy = entity.CreatedBy,
                Notes = $"Created document reference: {entity.DocumentRef}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<DocumentDto>(entity);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateDocumentAsync(UpdateDocumentCommand command)
    {
        var entity = await _context.Documents.FindAsync(command.DocumentId);

        if (entity == null) throw new Exception("Entity not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "documents",
            EntityId = entity.DocumentId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated document reference: {entity.DocumentRef}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(int id)
    {
        var entity = await _context.Documents.FindAsync(id);

        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "documents",
            EntityId = entity.DocumentId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted document reference: {entity.DocumentRef}"
        });

        await _context.SaveChangesAsync();
    }
}
