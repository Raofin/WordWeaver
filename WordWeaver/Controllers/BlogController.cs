using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController(IBlogService blogService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPosts(string? searchQuery, int pageIndex = 1, int pageSize = 10)
        {
            var response = await blogService.GetPosts(searchQuery, pageIndex, pageSize);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data)
                : BadRequest(response);
        }

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
    }
}
