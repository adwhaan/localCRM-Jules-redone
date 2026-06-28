using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Documents;

public class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        CreateMap<Document, DocumentDto>();
        CreateMap<CreateDocumentCommand, Document>();
        CreateMap<UpdateDocumentCommand, Document>();
    }
}

public class CreateDocumentCommand
{
    public string DocumentRef { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string Visibility { get; set; } = string.Empty;
    public string? Checksum { get; set; }
    public bool IsChecked { get; set; }

    public int? CompanyId { get; set; }
    public int? InteractionId { get; set; }
    public int? EngagementId { get; set; }
}

public class UpdateDocumentCommand : CreateDocumentCommand
{
    public int DocumentId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
