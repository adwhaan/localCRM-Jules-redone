using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Contact : SoftDeletableEntity
{
    public int ContactId { get; set; }
    public string? ContactRef { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LinkedinUrl { get; set; }
    public DateTime? Birthdate { get; set; }
    public string? ContactTags { get; set; }
    public int Rating { get; set; }
    public string? Sex { get; set; }
}
