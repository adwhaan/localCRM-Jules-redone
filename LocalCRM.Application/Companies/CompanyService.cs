using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LocalCRM.Application.Companies;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetAllAsync(bool includeDeleted = false);
    Task<CompanyDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateCompanyCommand command);
    Task UpdateAsync(UpdateCompanyCommand command);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}

public class CompanyService : ICompanyService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CompanyService(ILocalCRMContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<CompanyDto>> GetAllAsync(bool includeDeleted = false)
    {
        var query = _context.Companies.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetByIdAsync(int id)
    {
        return await _context.Companies
            .IgnoreQueryFilters()
            .Where(c => c.CompanyId == id)
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(CreateCompanyCommand command)
    {
        var entity = _mapper.Map<Company>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";

        _context.Companies.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Company",
            EntityId = entity.CompanyId,
            ActionType = "CREATE",
            PerformedBy = entity.CreatedBy,
            Notes = $"Created company: {entity.Name}"
        });

        await _context.SaveChangesAsync();
        return entity.CompanyId;
    }

    public async Task UpdateAsync(UpdateCompanyCommand command)
    {
        var entity = await _context.Companies.FindAsync(command.CompanyId);
        if (entity == null) throw new Exception("Entity not found");

        // Concurrency check
        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
            throw new Exception("Concurrency conflict");

        var summary = new StringBuilder();
        if (entity.Name != command.Name) summary.Append($"Name: {entity.Name} -> {command.Name}; ");
        if (entity.City != command.City) summary.Append($"City: {entity.City} -> {command.City}; ");
        if (entity.CompanyType != command.CompanyType) summary.Append($"Type: {entity.CompanyType} -> {command.CompanyType}; ");
        if (entity.Rating != command.Rating) summary.Append($"Rating: {entity.Rating} -> {command.Rating}; ");

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Company",
            EntityId = entity.CompanyId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = summary.Length > 0 ? summary.ToString() : "No changes detected"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Companies.FindAsync(id);
        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Company",
            EntityId = entity.CompanyId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Deleted company: {entity.Name}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        var entity = await _context.Companies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.CompanyId == id);
        if (entity == null) throw new Exception("Entity not found");

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUserService.Username ?? "system";

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "Company",
            EntityId = entity.CompanyId,
            ActionType = "RESTORE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Restored company: {entity.Name}"
        });

        await _context.SaveChangesAsync();
    }
}
