using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Services.Interfaces;

namespace WordWeaver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExcelController(IExcelService excelService, IAdminService adminService) : Controller
{
    [HttpGet("ExportUsersToExcel")]
    public async Task<IActionResult> ExportUsersToExcel(long userId = 0)
    {
        // Assume you have a method to retrieve user data from your repository or database
        var users = await adminService.GetUser(userId);

        if (users.StatusCode != HttpStatusCode.OK)
        {
            return BadRequest(users);
        }
        else if (users.Data == null)
        {
            return NotFound(users);
        }

        Task<Dtos.ResponseHelper<byte[]>> excelData = excelService.UsersListExcel(users.Data);
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"Users_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        //return Ok();
        return File(excelData.Result.Data, contentType, fileName);
    }
}
