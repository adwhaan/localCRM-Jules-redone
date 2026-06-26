namespace LocalCRM.Domain.Entities;

public class CompanyNoteLink
{
    public int CompanyId { get; set; }
    public int NoteId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Company Company { get; set; } = null!;
    public Note Note { get; set; } = null!;
}

public class CompanyDocumentLink
{
    public int CompanyId { get; set; }
    public int DocumentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Company Company { get; set; } = null!;
    public Document Document { get; set; } = null!;
}

public class ContactNoteLink
{
    public int ContactId { get; set; }
    public int NoteId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Contact Contact { get; set; } = null!;
    public Note Note { get; set; } = null!;
}

public class InteractionNoteLink
{
    public int InteractionId { get; set; }
    public int NoteId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Interaction Interaction { get; set; } = null!;
    public Note Note { get; set; } = null!;
}

public class InteractionDocumentLink
{
    public int InteractionId { get; set; }
    public int DocumentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Interaction Interaction { get; set; } = null!;
    public Document Document { get; set; } = null!;
}

public class EngagementCompanyLink
{
    public int EngagementId { get; set; }
    public int CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Engagement Engagement { get; set; } = null!;
    public Company Company { get; set; } = null!;
}

public class EngagementContactLink
{
    public int EngagementId { get; set; }
    public int ContactId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Engagement Engagement { get; set; } = null!;
    public Contact Contact { get; set; } = null!;
}

public class EngagementNoteLink
{
    public int EngagementId { get; set; }
    public int NoteId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Engagement Engagement { get; set; } = null!;
    public Note Note { get; set; } = null!;
}

public class EngagementDocumentLink
{
    public int EngagementId { get; set; }
    public int DocumentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Engagement Engagement { get; set; } = null!;
    public Document Document { get; set; } = null!;
}
