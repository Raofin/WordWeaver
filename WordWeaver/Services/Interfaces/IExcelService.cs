using WordWeaver.Dtos;

namespace WordWeaver.Services.Interfaces;

public interface IExcelService
{
    Task<ResponseHelper<byte[]>> UsersListExcel(List<UserListDto> users);
}