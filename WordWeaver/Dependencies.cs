using WordWeaver.Services.Core;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver;

public class Dependencies
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IAppSettingsService, AppSettingsService>();
        services.AddTransient<IMailService, MailService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticatedUser, TokenService>();
        services.AddScoped<ICloudService, CloudService>();
        services.AddScoped<ILoggerService, LoggerService>();

        services.AddScoped<IBlogService, BlogService>();

    }
}
