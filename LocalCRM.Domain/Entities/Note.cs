using LocalCRM.Domain.Common;

namespace LocalCRM.Domain.Entities;

public class Note : SoftDeletableEntity
{
    public int NoteId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;
}
