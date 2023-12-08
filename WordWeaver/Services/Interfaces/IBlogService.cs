using WordWeaver.Dtos;

namespace WordWeaver.Services.Interfaces;

public interface IBlogService
{
    Task<ResponseHelper> CreatePost(PostDto createPostDto);

    Task<ResponseHelper> UpdatePost(UpdatePostDto updatePostDto);

    Task<ResponseHelper> DeletePost(long postId);

    Task<ResponseHelper<PostDto>> GetPost(long postId);

    Task<ResponseHelper<PagedResult<PostPreviewDto>>> GetPosts(string? searchQuery, int pageIndex = 1, int pageSize = 10, bool currentUser = false);

    Task TrackPostView(long postId);

    Task<ResponseHelper> SaveReact(ReactDto dto);

    Task<ResponseHelper> SaveComment(CommentDto dto);

    Task<ResponseHelper<List<CommentFetchDto>>> FetchCommentsWithReplies(long blogId, long parentId = 0);

    Task<ResponseHelper> SaveBookmark(long postId);
}
