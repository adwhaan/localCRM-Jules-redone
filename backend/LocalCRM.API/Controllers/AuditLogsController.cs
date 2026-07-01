using LocalCRM.Application.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditLogDto>>> GetAuditLogs()
    {
        return await _auditLogService.GetAuditLogsAsync();
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<ActionResult<List<AuditLogDto>>> GetAuditLogsByEntity(string entityName, int entityId)
    {
        return await _auditLogService.GetAuditLogsByEntityAsync(entityName, entityId);
    }
}
