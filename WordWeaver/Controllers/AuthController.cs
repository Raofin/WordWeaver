using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Models;
using WordWeaver.Services.Auth;

namespace WordWeaver.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await authService.Login(model);

                return response.StatusCode == HttpStatusCode.OK
                    ? Ok(response)
                    : BadRequest(response);
            }

            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await authService.Register(model);

                return response.StatusCode == HttpStatusCode.OK
                    ? Ok(response)
                    : BadRequest(response);
            }

            return BadRequest(ModelState);
        }
    }
}
