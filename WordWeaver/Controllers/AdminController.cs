using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("Users")]
    public async Task<IActionResult> GetUsers(long userId = 0)
    {
        var response = await adminService.GetUser(userId);

        return response.StatusCode == HttpStatusCode.OK
            ? Ok(response.Data)
            : BadRequest(response);
    }
}
