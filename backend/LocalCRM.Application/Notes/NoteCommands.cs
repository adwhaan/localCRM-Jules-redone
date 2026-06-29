using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Notes;

public class NoteMappingProfile : Profile
{
    public NoteMappingProfile()
    {
        CreateMap<Note, NoteDto>();
        CreateMap<CreateNoteCommand, Note>();
        CreateMap<UpdateNoteCommand, Note>();
    }
}

public class CreateNoteCommand
{
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Visibility { get; set; } = string.Empty;

    public int? CompanyId { get; set; }
    public int? ContactId { get; set; }
    public int? InteractionId { get; set; }
    public int? EngagementId { get; set; }
}

public class UpdateNoteCommand : CreateNoteCommand
{
    public int NoteId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
