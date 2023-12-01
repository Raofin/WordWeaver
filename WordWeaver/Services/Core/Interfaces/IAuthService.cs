using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginDto model);

        Task<AuthResponse> Register(RegistrationDto model);
    }
}