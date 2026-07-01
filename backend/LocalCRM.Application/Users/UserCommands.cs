using AutoMapper;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Users;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Role.RoleName));
    }
}

public class CreateUserCommand
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
}

public class UpdateUserCommand
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
}
