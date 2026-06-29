using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Document : SoftDeletableEntity
{
    public int DocumentId { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Checksum { get; set; }
    public bool IsChecked { get; set; }
}
