using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CloudController(ICloudService cloudService) : ControllerBase
    {
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            return Ok(await cloudService.UploadFile(file));
        }

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            var file = await cloudService.DownloadFile(filename);
            return File(file, "application/octet-stream", filename);
        }
    }
}
