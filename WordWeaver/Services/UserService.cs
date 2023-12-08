using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Services;

public class UserService(WordWeaverContext context, IMapper mapper, IAuthenticatedUser authenticatedUser, ILoggerService log, ICloudService cloudService) : IUserService
{

    #region ### User Profile ###

    public async Task<ResponseHelper<ProfileDto>> GetProfile()
    {
        try
        {
            var profile = await (
                 from u in context.Users
                 join ud in context.UserDetails on u.UserId equals ud.UserId
                 where u.UserId == authenticatedUser.UserId && u.IsActive == true
                 select new ProfileDto {
                     ProfileId = ud.ProfileId,
                     UserId = u.UserId,
                     Username = u.Username,
                     Email = u.Email,
                     FullName = ud.FullName,
                     Bio = ud.Bio,
                     Birthday = ud.Birthday,
                     Country = ud.Country,
                     Phone = ud.Phone,
                     Website = ud.Website,
                     AvatarFileId = ud.AvatarFileId,
                     JoinedDate = u.CreatedAt,
                     Socials = (
                         from s in context.Socials
                         where s.UserId == authenticatedUser.UserId && s.IsActive == true
                         select new SocialDto {
                             SocialId = s.SocialId,
                             SocialName = s.SocialName,
                             SocialUrl = s.SocialUrl,
                             Description = s.Description,
                             IsActive = s.IsActive
                         }).ToList()
                 })
                 .FirstOrDefaultAsync();

            if (profile == null)
            {
                return new ResponseHelper<ProfileDto> {
                    Message = "User details was not found",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return new ResponseHelper<ProfileDto> {
                Data = profile,
                Message = "Profile retrieved successfully",
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<ProfileDto> {
                Message = ex.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper> SaveUserDetails(UserDetailsDto dto, IFormFile? avatarFile)
    {
        try
        {
            var response = new ResponseHelper();
            var userId = authenticatedUser.UserId;
            var extData = await context.UserDetails.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive == true);

            if (extData != null) // update
            {
                extData.FullName = dto.FullName ?? extData.FullName;
                extData.Bio = dto.Bio ?? extData.Bio;
                extData.Birthday = dto.Birthday ?? extData.Birthday;
                extData.Country = dto.Country ?? extData.Country;
                extData.Website = dto.Website ?? extData.Website;
                extData.Phone = dto.Phone ?? extData.Phone;
                extData.IsActive = dto.IsActive.HasValue ? dto.IsActive : extData.IsActive;

                // Update avatar
                if (avatarFile != null)
                {
                    var uploadedFile = await cloudService.UploadFile(avatarFile, userId);

                    if (extData.AvatarFileId > 0)
                    {
                        await cloudService.DeleteFile(extData.AvatarFileId.Value);
                    }

                    extData.AvatarFileId = uploadedFile?.Data?.FileId;
                }

                // Update socials
                if (dto.Socials != null && dto.Socials.Count != 0)
                {
                    foreach (var socialDto in dto.Socials)
                    {
                        var extSocial = context.Socials.FirstOrDefault(s => s.SocialId == socialDto.SocialId);

                        if (extSocial != null)
                        {
                            // Update existing social
                            extSocial.SocialName = socialDto.SocialName ?? extSocial.SocialName;
                            extSocial.SocialUrl = socialDto.SocialUrl ?? extSocial.SocialUrl;
                            extSocial.Description = socialDto.Description ?? extSocial.Description;
                        }
                        else
                        {
                            // Add new social
                            await context.Socials.AddAsync(new Social {
                                UserId = userId,
                                SocialName = socialDto.SocialName,
                                SocialUrl = socialDto.SocialUrl,
                                Description = socialDto.Description,
                            });
                        }
                    }
                }

                await context.SaveChangesAsync();

                response.Id = extData?.ProfileId;
                response.Message = "Profile updated successfully";
            }
            else // create
            {
                var data = new UserDetail {
                    UserId = userId,
                    FullName = dto.FullName,
                    Bio = dto.Bio,
                    Birthday = dto.Birthday,
                    Country = dto.Country,
                    Website = dto.Website,
                    Phone = dto.Phone,
                };

                // Upload avatar
                if (avatarFile != null)
                {
                    var uploadedFile = await cloudService.UploadFile(avatarFile, userId);

                    data.AvatarFileId = uploadedFile?.Data?.FileId;
                }

                await context.UserDetails.AddAsync(data);

                // Add socials
                if (dto.Socials != null && dto.Socials.Count != 0)
                {
                    var socials = dto.Socials.Select(s => new Social {
                        UserId = userId,
                        SocialName = s.SocialName,
                        SocialUrl = s.SocialUrl,
                        Description = s.Description,
                    });

                    await context.AddRangeAsync(socials);
                }

                await context.SaveChangesAsync();

                response.Id = data.ProfileId;
                response.Message = "Profile created successfully";
            }

            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper {
                Message = ex.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    #endregion ### User Profile ###

    #region ### User Posts ###

    public async Task<ResponseHelper<PostDto>> Posts()
    {
        try
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.UserId == authenticatedUser.UserId && x.IsActive == true);

            return new ResponseHelper<PostDto> {
                Message = "Post fetched successfully",
                StatusCode = HttpStatusCode.OK,
                Data = mapper.Map<PostDto>(post)
            };

        }
        catch (Exception ex)
        {
            return new ResponseHelper<PostDto> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<PostDto>> Bookmarks()
    {
        try
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.UserId == authenticatedUser.UserId && x.IsActive == true);

            return new ResponseHelper<PostDto> {
                Message = "Post fetched successfully",
                StatusCode = HttpStatusCode.OK,
                Data = mapper.Map<PostDto>(post)
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<PostDto> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<List<UserPostReactsDto>>> PostReacts()
    {
        try
        {
            var data = await context.Reacts
                .Where(x => x.UserId == authenticatedUser.UserId && x.IsActive == true && x.BlogId > 0)
                .Select(x => new UserPostReactsDto {
                    ReactId = x.ReactId,
                    BlogId = x.BlogId,
                    ReactEnumId = (Helpers.ReactTypes?)x.ReactEnumId
                }).ToListAsync();

            return new ResponseHelper<List<UserPostReactsDto>> {
                Message = "Post fetched successfully",
                StatusCode = HttpStatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<List<UserPostReactsDto>> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<List<UserCommentReactsDto>>> CommentReacts()
    {
        try
        {
            var data = await context.Reacts
                .Where(x => x.UserId == authenticatedUser.UserId && x.IsActive == true && x.CommentId > 0)
                .Select(x => new UserCommentReactsDto {
                    ReactId = x.ReactId,
                    CommentId = x.BlogId,
                    ReactEnumId = (Helpers.ReactTypes?)x.ReactEnumId
                }).ToListAsync();

            return new ResponseHelper<List<UserCommentReactsDto>> {
                Message = "Post fetched successfully",
                StatusCode = HttpStatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<List<UserCommentReactsDto>> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    #endregion ### User Posts ###
}
