using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Engagements;

public class EngagementMappingProfile : Profile
{
    public EngagementMappingProfile()
    {
        CreateMap<Engagement, EngagementDto>();
        CreateMap<CreateEngagementCommand, Engagement>();
        CreateMap<UpdateEngagementCommand, Engagement>();
    }
}

public class CreateEngagementCommand
{
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string? EngagementTags { get; set; }
    public string? Confidentiality { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
}

public class UpdateEngagementCommand : CreateEngagementCommand
{
    public int EngagementId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
