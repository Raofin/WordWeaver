using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordWeaver.Dtos;
using WordWeaver.Extensions;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class TestController(ITokenService tokenService, IMailService mailService, IAuthService authService, ILoggerService log) : ControllerBase
    {
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

        [HttpGet("GetMail")]
        public async Task<IActionResult> GetMail()
        {
            var email = await mailService.SendEmail(new EmailDto() {
                To = "zaidaminraofin@gmail.com",
                Subject = "Email subject",
                Body = "<h1>Hello World</h1>"
            }, 0, true);

            return Ok(email);
        }

        [HttpPost("username")]
        public async Task<IActionResult> Adwdqaw(string username)
        {
            var bol = await authService.IsUsernameUnique(username);

            return Ok(bol);
        }

        [HttpGet("TestString")]
        public async Task<IActionResult> TestStringAsync()
        {
            try
            {
                string commaSeparatedString = "10q,20,30,40";
                List<long> longValues = commaSeparatedString.ToLongList();

                var str = longValues.ToCommaSeparatedString();
                return Ok(str);

            } catch (Exception ex)
            {
                return Ok(await log.Error(ex));
            }

        }
    }
}
