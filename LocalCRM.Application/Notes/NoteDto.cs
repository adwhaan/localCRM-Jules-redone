namespace LocalCRM.Application.Notes;

public class NoteDto
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
