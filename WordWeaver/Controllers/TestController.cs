using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordWeaver.Services.Core;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(ITokenService tokenService, IMailService mailService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("GetToken")]
        public IActionResult Get()
        {
            return Ok(tokenService.DecodeJwt());
        }

        [HttpPost("PostToken")]
        public IActionResult Post()
        {
            return Ok("OK");
        }

        [AllowAnonymous]
        [HttpGet("GetMail")]
        public async Task<IActionResult> GetMail()
        {
            var email = await mailService.SendEmail(new Email() {
                To = "zaidaminraofin@gmail.com",
                Subject = "Email subject",
                Body = "<h1>Hello World</h1>"
            }, 0, true);

            return Ok(email);
        }
    }
}
