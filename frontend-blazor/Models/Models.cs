namespace LocalCRM.Blazor.Models;

public class LoginCommand
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public bool PasswordChangeRequired { get; set; }
}

public class CompanyDto
{
    public int CompanyId { get; set; }
    public string CompanyRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CompanyType { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ContactDto
{
    public int ContactId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Rating { get; set; }
}

public class InteractionDto
{
    public int InteractionId { get; set; }
    public DateTime InteractionDate { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public bool IsTask { get; set; }
}

public class EngagementDto
{
    public int EngagementId { get; set; }
    public string? EngagementRef { get; set; }
    public string EngagementStatus { get; set; } = string.Empty;
}

public class NoteDto
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;
}

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
}

public class AuditLogDto
{
    public int AuditId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}
