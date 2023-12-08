using WordWeaver.Dtos;

namespace WordWeaver.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ResponseHelper<List<UserListDto>>> GetUser(long userId);
    }
}