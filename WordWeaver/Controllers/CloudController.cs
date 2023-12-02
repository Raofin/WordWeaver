using System.Net;
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
            var response = await cloudService.UploadFile(file);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data) 
                : BadRequest(response);

        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            var response = await cloudService.UploadFiles(files);

            return response.StatusCode == HttpStatusCode.OK
                ? Ok(response.Data)
                : BadRequest(response);
        }

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            var file = await cloudService.DownloadFile(filename);
            return File(file, "application/octet-stream", filename);
        }
    }
}
