using WordWeaver.Models;

namespace WordWeaver.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginModel model);
        Task<AuthResponse> Register(RegistrationModel model);
    }
}