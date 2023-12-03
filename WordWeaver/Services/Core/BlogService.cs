using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Dtos;
using WordWeaver.Extensions;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class BlogService(WordWeaverContext context, IMapper mapper, IAuthenticatedUser authenticatedUser) : IBlogService
{
    public async Task<ResponseHelper> CreatePost(PostDto createPostDto)
    {
        try
        {
            var post = mapper.Map<Post>(createPostDto);
            post.UserId = authenticatedUser.UserId;

            await context.Posts.AddAsync(post);
            await context.SaveChangesAsync();

            return new ResponseHelper {
                Id = post.PostId,
                Message = "Post created successfully",
                StatusCode = HttpStatusCode.OK
            };

        } catch (Exception ex)
        {
            return new ResponseHelper {
                Id = createPostDto.PostId,
                Message = ex.Message,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper> UpdatePost(UpdatePostDto updatePostDto)
    {
        try
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.IsActive == true && x.PostId == updatePostDto.PostId);

            if (post == null)
            {
                return new ResponseHelper {
                    Id = updatePostDto.PostId,
                    Message = "Post not found",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            post.Title = updatePostDto.Title?? post.Title;
            post.Description = updatePostDto.Description?? post.Description;
            post.Text = updatePostDto.Text?? post.Text;
            post.FileIds = updatePostDto.FileIds.ToCommaSeparatedString() ?? post.FileIds;
            post.IsPublished = updatePostDto.IsPublished?? post.IsPublished;

            await context.SaveChangesAsync();

            return new ResponseHelper {
                Message = "Post updated successfully",
                StatusCode = HttpStatusCode.OK,
                Id = post.PostId
            };

        } catch (Exception ex)
        {
            return new ResponseHelper {
                Id = updatePostDto.PostId,
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper> DeletePost(long postId)
    {
        try
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == postId && x.IsActive == true);

            if (post == null)
            {
                return new ResponseHelper {
                    Id = postId,
                    Message = "Post not found",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            post.IsActive = false;
            await context.SaveChangesAsync();

            return new ResponseHelper {
                Id = postId,
                Message = "Post deleted successfully",
                StatusCode = HttpStatusCode.OK
            };

        } catch (Exception ex)
        {
            return new ResponseHelper {
                Id = postId,
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<PostDto>> GetPost(long postId)
    {
        try
        {
            var post = await context.Posts.FirstOrDefaultAsync(x => x.PostId == postId && x.IsActive == true);

            return new ResponseHelper<PostDto> {
                Message = "Post fetched successfully",
                StatusCode = HttpStatusCode.OK,
                Data = mapper.Map<PostDto>(post)
            };

        } catch (Exception ex)
        {
            return new ResponseHelper<PostDto> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseHelper<PagedResult<PostPreviewDto>>> GetPosts(string? searchQuery, int pageIndex = 1, int pageSize = 10)
    {
        try
        {
            // Calculate the number of items to skip based on the page index and page size
            var skipAmount = (pageIndex - 1) * pageSize;

            // Retrieve posts with pagination
            var posts = await context.Posts
                .Where(x => x.IsActive == true && (string.IsNullOrEmpty(searchQuery) || x.Title.Contains(searchQuery) || x.Description.Contains(searchQuery)))
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skipAmount).Take(pageSize)
                .Select(x => new PostPreviewDto {
                    PostId = x.PostId,
                    UserId = x.UserId,
                    Title = x.Title,
                    Description = x.Description
                })
                .AsNoTracking().ToListAsync();

            // Count total number of posts without pagination
            var totalPostsCount = await context.Posts
                .CountAsync(x => x.IsActive == true && (string.IsNullOrEmpty(searchQuery) || x.Title.Contains(searchQuery) || x.Description.Contains(searchQuery)));

            // Create a PagedResult to encapsulate the result and pagination information
            var pagedResult = new PagedResult<PostPreviewDto> {
                Data = posts,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalPostsCount,
                TotalPages = (int)Math.Ceiling((double)totalPostsCount / pageSize)
            };

            return new ResponseHelper<PagedResult<PostPreviewDto>> {
                Message = pagedResult.Data.Count > 0 ? "Posts fetched successfully" : "No posts found",
                StatusCode = pagedResult.Data.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                Data = pagedResult
            };

        } catch (Exception ex)
        {
            return new ResponseHelper<PagedResult<PostPreviewDto>> {
                Message = $"Error: {ex.Message}",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}
