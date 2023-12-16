using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Services.Pdf;

namespace WordWeaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController(IPdfService pdfService) : ControllerBase
    {
        [HttpGet("PostPdf")]
        public async Task<IActionResult> PostPdf(long postId)
        {
            var response = await pdfService.PostPdf(postId);

            return response.StatusCode == HttpStatusCode.OK && response.Data != null
                ? File(response.Data, "application/pdf")
                : BadRequest(response);
        }
    }
}
