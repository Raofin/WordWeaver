using WordWeaver.Dtos;

namespace WordWeaver.Services.Interfaces;

public interface IUserService
{
    Task<ResponseHelper> SaveUserDetails(UserDetailsDto dto, IFormFile? avatarFile);

    Task<ResponseHelper<ProfileDto>> GetProfile();

    Task<ResponseHelper<PostDto>> Bookmarks();

    Task<ResponseHelper<List<UserPostReactsDto>>> PostReacts();

    Task<ResponseHelper<List<UserCommentReactsDto>>> CommentReacts();
}