using AutoMapper;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;

namespace WordWeaver.Mappings;

public class AuthProfiles : Profile
{
    public AuthProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Salt, opt => opt.Ignore());

        CreateMap<UserRole, UserRoleDto>();
        CreateMap<UserRoleDto, UserRole>();
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();
    }
}
