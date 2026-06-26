using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Interactions;

public interface IInteractionService
{
    Task<List<InteractionDto>> GetInteractionsAsync();
    Task<InteractionDto?> GetInteractionByIdAsync(int id);
    Task<InteractionDto> CreateInteractionAsync(CreateInteractionCommand command);
    Task UpdateInteractionAsync(UpdateInteractionCommand command);
    Task DeleteInteractionAsync(int id);
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

    public async Task<List<InteractionDto>> GetInteractionsAsync()
    {
        return await _context.Interactions
            .ProjectTo<InteractionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<InteractionDto?> GetInteractionByIdAsync(int id)
    {
        return await _context.Interactions
            .Where(i => i.InteractionId == id)
            .ProjectTo<InteractionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<InteractionDto> CreateInteractionAsync(CreateInteractionCommand command)
    {
        using var transaction = await ((DbContext)_context).Database.BeginTransactionAsync();
        try
        {
            var entity = _mapper.Map<Interaction>(command);
            entity.CreatedBy = _currentUserService.Username ?? "system";
            entity.CreatedAt = DateTime.UtcNow;

            _context.Interactions.Add(entity);
            await _context.SaveChangesAsync();

            // Linkage logic
            if (command.ContactId.HasValue || command.CompanyId.HasValue || command.EngagementId.HasValue)
            {
                var link = new InteractionLink
                {
                    InteractionId = entity.InteractionId,
                    ContactId = command.ContactId,
                    CompanyId = command.CompanyId,
                    EngagementId = command.EngagementId,
                    CreatedBy = entity.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                if (command.ContactId.HasValue && command.CompanyId.HasValue)
                {
                    throw new Exception("ContactId and CompanyId are mutually exclusive");
                }

                _context.InteractionLinks.Add(link);
            }

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "interactions",
                EntityId = entity.InteractionId,
                ActionType = "CREATE",
                PerformedBy = entity.CreatedBy,
                Notes = $"Created interaction: {entity.Subject}"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return _mapper.Map<InteractionDto>(entity);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateInteractionAsync(UpdateInteractionCommand command)
    {
        var entity = await _context.Interactions.FindAsync(command.InteractionId);

        if (entity == null) throw new Exception("Entity not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "interactions",
            EntityId = entity.InteractionId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated interaction: {entity.Subject}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteInteractionAsync(int id)
    {
        var entity = await _context.Interactions.FindAsync(id);

        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "interactions",
            EntityId = entity.InteractionId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted interaction: {entity.Subject}"
        });

        await _context.SaveChangesAsync();
    }
}
