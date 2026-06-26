using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Engagement : SoftDeletableEntity
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string? EngagementTags { get; set; }
    public string? Confidentiality { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
}
