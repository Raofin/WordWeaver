using B2Net;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class CloudService(IAppSettingsService appSettings, WordWeaverContext context) : ICloudService
{
    public async Task<ResponseHelper<List<CloudFile>>> GetFiles(string searchQuery = "")
    {
        try
        {
            var cloudFiles = await context.CloudFiles
                .Where(x => x.IsActive == true && (string.IsNullOrEmpty(searchQuery) || x.Name.Contains(searchQuery) || x.Extension.Contains(searchQuery)))
                .ToListAsync();

            return new ResponseHelper<List<CloudFile>> {
                Message = "Files list retrieved successfully",
                StatusCode = HttpStatusCode.OK,
                Data = cloudFiles
            };

        } catch (Exception ex)
        {
            return new ResponseHelper<List<CloudFile>> {
                Message = ex.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<CloudFile>> UploadFile(IFormFile file, long? userId = null, string? filename = null)
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
                    UserId = userId
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

    public async Task<ResponseHelper<List<CloudFile>>> UploadFiles(IFormFileCollection files, long? userId = null)
    {
        using (var memoryStream = new MemoryStream())
        {
            try
            {
                var cloudFiles = new List<CloudFile>();

                foreach (var file in files)
                {
                    var guid = Guid.NewGuid().ToString()[..6];
                    var filename = guid + "_" + file.FileName.Trim();

                    file.CopyTo(memoryStream);

                    var client = new B2Client(appSettings.B2KeyId, appSettings.B2AppKey);
                    var fileBytes = memoryStream.ToArray();
                    var results = await client.Files.Upload(fileBytes, filename, appSettings.B2BucketId);

                    var cloudFile = new CloudFile {
                        Name = results.FileName,
                        Extension = Path.GetExtension(file.FileName),
                        Size = results.ContentLength,
                        UploadedAt = results.UploadTimestampDate,
                        UserId = userId
                    };

                    cloudFiles.Add(cloudFile);
                }

                await context.CloudFiles.AddRangeAsync(cloudFiles);
                await context.SaveChangesAsync();

                return new ResponseHelper<List<CloudFile>> {
                    Message = "Files uploaded successfully",
                    StatusCode = HttpStatusCode.OK,
                    Data = cloudFiles
                };
            } catch (Exception ex)
            {
                return new ResponseHelper<List<CloudFile>> {
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

    public async Task<ResponseHelper> DeleteFile(long fileId)
    {
        try
        {
            var cloudFile = await context.CloudFiles.Where(x => x.FileId == fileId).FirstOrDefaultAsync();

            if (cloudFile != null)
            {
                cloudFile.IsActive = false;
                await context.SaveChangesAsync();
            }

            return new ResponseHelper {
                Message = "File deleted successfully",
                StatusCode = HttpStatusCode.OK
            };

        } catch (Exception ex)
        {
            return new ResponseHelper {
                Message = ex.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}