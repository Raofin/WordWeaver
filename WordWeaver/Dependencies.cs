using WordWeaver.Services;
using WordWeaver.Services.Auth;

namespace WordWeaver
{
    public class Dependencies
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<TokenService>();
        }
    }
}
