#pragma warning disable CS8603 // Possible null reference return.

using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class AppSettingsService(IConfiguration configuration) : IAppSettingsService
{
    public string ConnectionString
    {
        get => configuration.GetConnectionString("DefaultConnection");
    }

    public string JwtKey
    {
        get => configuration.GetValue("Jwt:Key", string.Empty);
    }

    public string JwtIssuer
    {
        get => configuration.GetValue("Jwt:Issuer", string.Empty);
    }

    public string JwtAudience
    {
        get => configuration.GetValue("Jwt:Audience", string.Empty);
    }

    public string SmtpHost
    {
        get => configuration.GetValue("SmtpSettings:Host", string.Empty);
    }

    public int SmtpPort
    {
        get => configuration.GetValue("SmtpSettings:Port", 0);
    }

    public bool SmtpEnableSsl
    {
        get => configuration.GetValue("SmtpSettings:EnableSsl", false);
    }

    public string SmtpUsername
    {
        get => configuration.GetValue("SmtpSettings:Username", string.Empty);
    }

    public string SmtpPassword
    {
        get => configuration.GetValue("SmtpSettings:Password", string.Empty)?.Replace(" ", string.Empty);
    }
}
