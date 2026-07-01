using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Contacts;

public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        CreateMap<Contact, ContactDto>();
        CreateMap<CreateContactCommand, Contact>();
        CreateMap<UpdateContactCommand, Contact>();
    }
}

public class CreateContactCommand
{
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

public class UpdateContactCommand : CreateContactCommand
{
    public int ContactId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
