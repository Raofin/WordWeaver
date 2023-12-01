using B2Net;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class CloudService(IAppSettingsService appSettings, WordWeaverContext context) : ICloudService
{
    public async Task<ResponseHelper<CloudFile>> UploadFile(IFormFile file, long uploadedBy = 0, string? filename = null)
    {
        using (var memoryStream = new MemoryStream())
        {
            try
            {
                var guid = Guid.NewGuid().ToString()[..6];
                filename ??= file.FileName;
                filename = guid + "_" + filename.Trim();

                file.CopyTo(memoryStream);

                var client = new B2Client(appSettings.B2KeyId, appSettings.B2AppKey);
                var fileBytes = memoryStream.ToArray();
                var results = await client.Files.Upload(fileBytes, filename, appSettings.B2BucketId);

                var cloudFile = new CloudFile {
                    Name = results.FileName,
                    Extension = Path.GetExtension(file.FileName),
                    Size = results.ContentLength,
                    UploadedAt = results.UploadTimestampDate,
                    UploadedBy = uploadedBy
                };
                
                await context.CloudFiles.AddAsync(cloudFile);
                await context.SaveChangesAsync();

                return new ResponseHelper<CloudFile> {
                    Message = "File uploaded successfully",
                    StatusCode = HttpStatusCode.OK,
                    Data = cloudFile
                };

            } catch (Exception ex)
            {
                return new ResponseHelper<CloudFile> {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }

    public async Task<byte[]> DownloadFile(string filename)
    {
        var client = new B2Client(appSettings.B2KeyId, appSettings.B2AppKey);
        var fileByte = await client.Files.DownloadByName(filename, appSettings.B2BucketName);

        return fileByte.FileData;
    }
}
