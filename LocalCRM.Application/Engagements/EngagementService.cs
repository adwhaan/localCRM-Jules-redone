using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Engagements;

public interface IEngagementService
{
    Task<List<EngagementDto>> GetEngagementsAsync();
    Task<EngagementDto?> GetEngagementByIdAsync(int id);
    Task<EngagementDto> CreateEngagementAsync(CreateEngagementCommand command);
    Task UpdateEngagementAsync(UpdateEngagementCommand command);
    Task DeleteEngagementAsync(int id);
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

    public async Task<List<EngagementDto>> GetEngagementsAsync()
    {
        return await _context.Engagements
            .ProjectTo<EngagementDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EngagementDto?> GetEngagementByIdAsync(int id)
    {
        return await _context.Engagements
            .Where(e => e.EngagementId == id)
            .ProjectTo<EngagementDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<EngagementDto> CreateEngagementAsync(CreateEngagementCommand command)
    {
        var entity = _mapper.Map<Engagement>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        entity.CreatedAt = DateTime.UtcNow;

        _context.Engagements.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "engagements",
            EntityId = entity.EngagementId,
            ActionType = "CREATE",
            PerformedBy = entity.CreatedBy,
            Notes = $"Created engagement: {entity.EngagementRef}"
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<EngagementDto>(entity);
    }

    public async Task UpdateEngagementAsync(UpdateEngagementCommand command)
    {
        var entity = await _context.Engagements.FindAsync(command.EngagementId);

        if (entity == null) throw new Exception("Entity not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "engagements",
            EntityId = entity.EngagementId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated engagement: {entity.EngagementRef}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEngagementAsync(int id)
    {
        var entity = await _context.Engagements.FindAsync(id);

        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "engagements",
            EntityId = entity.EngagementId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted engagement: {entity.EngagementRef}"
        });

        await _context.SaveChangesAsync();
    }
}
