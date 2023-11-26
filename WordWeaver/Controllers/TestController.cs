using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WordWeaver.Services;

namespace WordWeaver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(TokenService tokenService) : ControllerBase
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
    }
}
