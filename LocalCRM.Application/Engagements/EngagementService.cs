using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Engagements;

public interface IEngagementService
{
    Task<List<EngagementDto>> GetAllAsync(bool includeDeleted = false);
    Task<EngagementDto?> GetByIdAsync(int id);
    Task<EngagementDto> CreateAsync(CreateEngagementCommand command);
    Task UpdateAsync(UpdateEngagementCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}

public class EngagementService : IEngagementService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public EngagementService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<EngagementDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Engagements.AsQueryable();
        if (includeDeleted) query = query.IgnoreQueryFilters();
        return await query.ProjectTo<EngagementDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<EngagementDto?> GetByIdAsync(int id)
    {
        return await _context.Engagements.IgnoreQueryFilters()
            .Where(e => e.EngagementId == id)
            .ProjectTo<EngagementDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<EngagementDto> CreateAsync(CreateEngagementCommand command)
    {
        var entity = _mapper.Map<Engagement>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        _context.Engagements.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Engagement", EntityId = entity.EngagementId, ActionType = "CREATE",
            PerformedBy = entity.CreatedBy, Notes = $"Created engagement: {entity.EngagementRef}"
        });
        await _context.SaveChangesAsync();
        return _mapper.Map<EngagementDto>(entity);
    }

    public async Task UpdateAsync(UpdateEngagementCommand command)
    {
        var entity = await _context.Engagements.FindAsync(command.EngagementId);
        if (entity == null) throw new Exception("Entity not found");
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt) throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.EngagementRef != command.EngagementRef) summary.Append($"Ref: {entity.EngagementRef} -> {command.EngagementRef}; ");
        if (entity.EngagementStatus != command.EngagementStatus) summary.Append($"Status: {entity.EngagementStatus} -> {command.EngagementStatus}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Engagement", EntityId = entity.EngagementId, ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy, Notes = summary.Length > 0 ? summary.ToString() : "No changes"
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Engagements.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Engagement", EntityId = entity.EngagementId, ActionType = "SOFT_DELETE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Deleted engagement: {entity.EngagementRef}"
        });
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Engagements.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.EngagementId == id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Engagement", EntityId = entity.EngagementId, ActionType = "RESTORE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Restored engagement: {entity.EngagementRef}"
        });
        await _context.SaveChangesAsync();
    }
}
