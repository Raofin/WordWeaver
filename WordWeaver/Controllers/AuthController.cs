using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WordWeaver.Dtos;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
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

        [HttpGet("SendOtp")]
        public async Task<IActionResult> GetOtp(string email)
        {
            return await authService.SendOtp(email)
                ? Ok(new { message = "OTP sent successfully." })
                : BadRequest(new { message = "Email must be unique." });
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(VerifyOtpDto model)
        {
            if (ModelState.IsValid)
            {
                return await authService.VerifyEmail(model.Email, model.Otp)
                    ? Ok(new { message = "Email is verified." })
                    : BadRequest(new { message = "Otp is invalid or expired." });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("IsUsernameUnique")]
        public async Task<IActionResult> IsUsernameUnique(string username)
        {
            return await authService.IsUsernameUnique(username)
                ? Ok(new { message = "Username is unique." })
                : BadRequest(new { message = "Username must be unique." });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationDto model)
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
