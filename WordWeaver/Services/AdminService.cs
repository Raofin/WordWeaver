using Microsoft.EntityFrameworkCore;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Services;

public class AdminService(WordWeaverContext context, ILoggerService log) : IAdminService
{
    public async Task<ResponseHelper<List<UserListDto>>> GetUser(long userId)
    {
        try
        {
            var data = await (
                from u in context.Users
                where (userId <=0 || u.UserId == userId)
                select new UserListDto {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    JoinedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    IsActive = u.IsActive,
                    UserDetails = (
                        from ud in context.UserDetails where ud.UserId == u.UserId
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
                                where s.UserId == u.UserId
                                select new SocialDto {
                                    SocialId = s.SocialId,
                                    SocialName = s.SocialName,
                                    SocialUrl = s.SocialUrl,
                                    Description = s.Description,
                                    IsActive = s.IsActive
                                }).ToList()
                        }).FirstOrDefault(),
                    UserRoles = (
                        from ur in context.UserRoles
                        join r in context.RoleEnums on ur.RoleId equals r.RoleId
                        where ur.UserId == u.UserId
                        select new UserRoleListDto {
                            UserRoleId = ur.UserRoleId,
                            RoleId = ur.RoleId,
                            RoleName = r.RoleName,
                            CreatedAt = ur.CreatedAt,
                            UpdatedAt = ur.UpdatedAt,
                            IsActive = ur.IsActive
                        }).ToList()
                })
                .ToListAsync();

            return new ResponseHelper<List<UserListDto>> {
                StatusCode = HttpStatusCode.OK,
                Data = data
            };
        }
        catch (Exception ex)
        {
            await log.Error(ex);

            return new ResponseHelper<List<UserListDto>> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

    }
}
