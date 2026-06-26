using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Interactions;

public class InteractionMappingProfile : Profile
{
    public InteractionMappingProfile()
    {
        CreateMap<Interaction, InteractionDto>();
        CreateMap<CreateInteractionCommand, Interaction>();
        CreateMap<UpdateInteractionCommand, Interaction>();
    }
}

public class CreateInteractionCommand
{
    public DateTime InteractionDate { get; set; }
    public TimeSpan? InteractionTime { get; set; }
    public string? Direction { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string State { get; set; } = string.Empty;
    public int? PrevInteractionId { get; set; }
    public string? InteractionTags { get; set; }
    public bool IsTask { get; set; }

    public int? ContactId { get; set; }
    public int? CompanyId { get; set; }
    public int? EngagementId { get; set; }
}

public class UpdateInteractionCommand : CreateInteractionCommand
{
    public int InteractionId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
