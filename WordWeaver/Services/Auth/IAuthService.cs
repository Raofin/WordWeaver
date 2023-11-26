using WordWeaver.Dtos;

namespace WordWeaver.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginDto model);

        Task<AuthResponse> Register(RegistrationDto model);
    }
}