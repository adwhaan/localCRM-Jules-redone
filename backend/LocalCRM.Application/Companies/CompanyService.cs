using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Companies;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetCompaniesAsync();
    Task<CompanyDto?> GetCompanyByIdAsync(int id);
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyCommand command);
    Task UpdateCompanyAsync(UpdateCompanyCommand command);
    Task DeleteCompanyAsync(int id);
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

    public async Task<List<CompanyDto>> GetCompaniesAsync()
    {
        return await _context.Companies
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies
            .Where(c => c.CompanyId == id)
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyCommand command)
    {
        var entity = _mapper.Map<Company>(command);
        entity.CreatedBy = _currentUserService.Username ?? "system";
        entity.CreatedAt = DateTime.UtcNow;

        _context.Companies.Add(entity);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "companies",
            EntityId = entity.CompanyId,
            ActionType = "CREATE",
            PerformedBy = entity.CreatedBy,
            Notes = $"Created company {entity.Name}"
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<CompanyDto>(entity);
    }

    public async Task UpdateCompanyAsync(UpdateCompanyCommand command)
    {
        var entity = await _context.Companies.FindAsync(command.CompanyId);

        if (entity == null)
        {
            throw new Exception("Entity not found");
        }

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != command.UpdatedAt)
        {
            throw new Exception("Concurrency conflict");
        }

        _mapper.Map(command, entity);
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "companies",
            EntityId = entity.CompanyId,
            ActionType = "UPDATE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Updated company {entity.Name}"
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(int id)
    {
        var entity = await _context.Companies.FindAsync(id);

        if (entity == null)
        {
            throw new Exception("Entity not found");
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUserService.Username ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "companies",
            EntityId = entity.CompanyId,
            ActionType = "SOFT_DELETE",
            PerformedBy = entity.UpdatedBy,
            Notes = $"Soft deleted company {entity.Name}"
        });

        await _context.SaveChangesAsync();
    }
}
