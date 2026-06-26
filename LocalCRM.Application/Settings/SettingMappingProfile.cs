using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Settings;

public class SettingMappingProfile : Profile
{
    public SettingMappingProfile()
    {
        CreateMap<Setting, SettingDto>();
    }
}
