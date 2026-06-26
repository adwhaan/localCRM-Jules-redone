using AutoMapper;
using AutoMapper.QueryableExtensions;
using LocalCRM.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.AuditLogs;

public interface IAuditLogService
{
    Task<List<AuditLogDto>> GetAuditLogsAsync();
    Task<List<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName, int entityId);
}

public class AuditLogService : IAuditLogService
{
    private readonly ILocalCRMContext _context;
    private readonly IMapper _mapper;

    public AuditLogService(ILocalCRMContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AuditLogDto>> GetAuditLogsAsync()
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.PerformedAt)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<AuditLogDto>> GetAuditLogsByEntityAsync(string entityName, int entityId)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.PerformedAt)
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
