using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginDto model);
        Task<AuthResponse> Register(RegistrationDto model);
        Task<bool> IsUsernameUnique(string username);
        Task<bool> SendOtp(string email);
        Task<bool> VerifyEmail(string email, string otp);
    }
}