using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.JsonMultipartFormDataSupport.Models;
using System.Net;
using WordWeaver.Dtos;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("Profile")]
    public async Task<IActionResult> GetProfile()
    {
        var response = await userService.GetProfile();

        return response.StatusCode == HttpStatusCode.OK
            ? Ok(response.Data)
            : BadRequest(response);
    }

    [HttpPost("Profile")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SaveUserDetails([FromForm] MultipartFormData<UserDetailsDto> data)
    {
        if (ModelState.IsValid)
        {
            var response = await userService.SaveUserDetails(data.Json, data.File);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response)
                : BadRequest(response);
        }

        return BadRequest(ModelState);
    }

    [HttpPost("Bookmarks")]
    public async Task<IActionResult> Bookmarks()
    {
        var response = await userService.Bookmarks();

        return response.StatusCode == HttpStatusCode.OK
            ? Ok(response.Data)
            : BadRequest(response);
    }

    [HttpPost("PostReacts")]
    public async Task<IActionResult> PostReacts()
    {
        var response = await userService.PostReacts();

        return response.StatusCode == HttpStatusCode.OK
            ? Ok(response.Data)
            : BadRequest(response);
    }

    [HttpPost("CommentReacts")]
    public async Task<IActionResult> CommentReacts()
    {
        var response = await userService.CommentReacts();

        return response.StatusCode == HttpStatusCode.OK
            ? Ok(response.Data)
            : BadRequest(response);
    }
}