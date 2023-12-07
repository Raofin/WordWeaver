using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Services;

public class UserService(WordWeaverContext context, IAuthenticatedUser authenticatedUser, ILoggerService log) : IUserService
{
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

    public async Task<ResponseHelper> SaveUserDetails(UserDetailsDto dto)
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
                //extData.AvatarFileId = dto.AvatarFileId ?? extData.AvatarFileId;
                extData.IsActive = dto.IsActive.HasValue ? dto.IsActive : extData.IsActive;

                await context.SaveChangesAsync();

                response.Id = extData.ProfileId;
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
                    //AvatarFileId = dto.AvatarFileId,
                    IsActive = true
                };

                await context.UserDetails.AddAsync(data);
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
}
