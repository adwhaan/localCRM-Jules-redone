namespace LocalCRM.Domain.Entities;

public class CompanyContactLink
{
    public int CompanyId { get; set; }
    public int ContactId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Company Company { get; set; } = null!;
    public Contact Contact { get; set; } = null!;
}
