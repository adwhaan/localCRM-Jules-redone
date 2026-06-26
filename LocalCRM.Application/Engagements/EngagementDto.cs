namespace LocalCRM.Application.Engagements;

public class EngagementDto
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string? Description { get; set; }
    public string? EngagementTags { get; set; }
    public string? Confidentiality { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
