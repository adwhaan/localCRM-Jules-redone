namespace LocalCRM.Domain.Entities;

public class InteractionLink
{
    public int InteractionId { get; set; }
    public int? ContactId { get; set; }
    public int? CompanyId { get; set; }
    public int? EngagementId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Interaction Interaction { get; set; } = null!;
    public Contact? Contact { get; set; }
    public Company? Company { get; set; }
    public Engagement? Engagement { get; set; }
}
