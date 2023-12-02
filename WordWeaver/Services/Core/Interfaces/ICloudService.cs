using WordWeaver.Data.Entity;
using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces;

public interface ICloudService
{
    Task<ResponseHelper<List<CloudFile>>> GetFiles(string searchQuery = "");
    Task<ResponseHelper<CloudFile>> UploadFile(IFormFile file, long? userId = null, string? filename = null);
    Task<ResponseHelper<List<CloudFile>>> UploadFiles(IFormFileCollection files, long? userId = null);
    Task<byte[]> DownloadFile(string filename);
    Task<ResponseHelper> DeleteFile(long fileId);
}