using WordWeaver.Dtos;

namespace WordWeaver.Services.Interfaces;

public interface IUserService
{
    Task<ResponseHelper> SaveUserDetails(UserDetailsDto dto);

    Task<ResponseHelper<ProfileDto>> GetProfile();
}