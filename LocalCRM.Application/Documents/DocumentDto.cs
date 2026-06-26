namespace LocalCRM.Application.Documents;

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Checksum { get; set; }
    public bool IsChecked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
