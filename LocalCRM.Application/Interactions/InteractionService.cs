using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Interactions;

public interface IInteractionService
{
    Task<List<InteractionDto>> GetAllAsync(bool includeDeleted = false);
    Task<InteractionDto?> GetByIdAsync(int id);
    Task<InteractionDto> CreateAsync(CreateInteractionCommand command);
    Task UpdateAsync(UpdateInteractionCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}

public class InteractionService : IInteractionService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public InteractionService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<InteractionDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Interactions.AsQueryable();
        if (includeDeleted) query = query.IgnoreQueryFilters();
        return await query.ProjectTo<InteractionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<InteractionDto?> GetByIdAsync(int id)
    {
        return await _context.Interactions.IgnoreQueryFilters()
            .Where(i => i.InteractionId == id)
            .ProjectTo<InteractionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<InteractionDto> CreateAsync(CreateInteractionCommand command)
    {
        var entity = _mapper.Map<Interaction>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        _context.Interactions.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Interaction", EntityId = entity.InteractionId, ActionType = "CREATE",
            PerformedBy = entity.CreatedBy, Notes = $"Created interaction: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
        return _mapper.Map<InteractionDto>(entity);
    }

    public async Task UpdateAsync(UpdateInteractionCommand command)
    {
        var entity = await _context.Interactions.FindAsync(command.InteractionId);
        if (entity == null) throw new Exception("Entity not found");
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt) throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.Subject != command.Subject) summary.Append($"Subject: {entity.Subject} -> {command.Subject}; ");
        if (entity.State != command.State) summary.Append($"State: {entity.State} -> {command.State}; ");
        if (entity.IsTask != command.IsTask) summary.Append($"IsTask: {entity.IsTask} -> {command.IsTask}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Interaction", EntityId = entity.InteractionId, ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy, Notes = summary.Length > 0 ? summary.ToString() : "No changes"
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Interactions.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Interaction", EntityId = entity.InteractionId, ActionType = "SOFT_DELETE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Deleted interaction: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Interactions.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.InteractionId == id);
        if (entity == null) throw new Exception("Entity not found");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.AuditLogs.Add(new AuditLog {
            EntityName = "Interaction", EntityId = entity.InteractionId, ActionType = "RESTORE",
            PerformedBy = _currentUserService.Username ?? "system", Notes = $"Restored interaction: {entity.Subject}"
        });
        await _context.SaveChangesAsync();
    }
}
