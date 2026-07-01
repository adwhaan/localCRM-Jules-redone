using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Tags;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        CreateMap<Tag, TagDto>();
    }
}
