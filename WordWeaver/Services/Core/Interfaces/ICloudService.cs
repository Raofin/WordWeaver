using WordWeaver.Data.Entity;
using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces;

public interface ICloudService
{
    Task<byte[]> DownloadFile(string filename);
    Task<ResponseHelper<CloudFile>> UploadFile(IFormFile file, long uploadedBy = 0, string? filename = null);
}