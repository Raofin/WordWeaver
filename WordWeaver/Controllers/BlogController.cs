using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Dtos;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController(IBlogService blogService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPosts(string? searchQuery, int pageIndex = 1, int pageSize = 10)
        {
            var response = await blogService.GetPosts(searchQuery, pageIndex, pageSize);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data)
                : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetBlog(long postId)
        {
            var response = await blogService.GetPost(postId);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data)
                : BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog(PostDto createPostDto)
        {
            if (ModelState.IsValid)
            {
                var response = await blogService.CreatePost(createPostDto);

                return response.StatusCode == HttpStatusCode.OK
                    ? Ok(response)
                    : BadRequest(response);
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlog(UpdatePostDto updatePostDto)
        {
            if (ModelState.IsValid)
            {
                var response = await blogService.UpdatePost(updatePostDto);

                return response.StatusCode == HttpStatusCode.OK
                    ? Ok(response)
                    : BadRequest(response);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeleteBlog(long postId)
        {
            var response = await blogService.DeletePost(postId);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response)
                : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("TrackPostView")]
        public async Task<IActionResult> TrackPostView(long postId)
        {
            await blogService.TrackPostView(postId);
            return Ok();
        }

        [HttpPost("SaveReact")]
        public async Task<IActionResult> SaveReact(ReactDto reactDto)
        {
            var response = await blogService.SaveReact(reactDto);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response)
                : BadRequest(response);
        }

        [HttpPost("SaveComment")]
        public async Task<IActionResult> SaveComment(CommentDto commentDto)
        {
            var response = await blogService.SaveComment(commentDto);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response)
                : BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("CommentsWithReplies")]
        public async Task<IActionResult> CommentsWithReplies(long blogId)
        {
            var response = await blogService.FetchCommentsWithReplies(blogId);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data)
                : BadRequest(response);
        }
    }
}
