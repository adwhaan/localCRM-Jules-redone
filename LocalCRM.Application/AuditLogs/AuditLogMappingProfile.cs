using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.AuditLogs;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<AuditLog, AuditLogDto>();
    }
}
