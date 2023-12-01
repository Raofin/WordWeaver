namespace WordWeaver.Services.Core.Interfaces;

public interface IAppSettingsService
{
    string ConnectionString { get; }
    string JwtAudience { get; }
    string JwtIssuer { get; }
    string JwtKey { get; }
    bool SmtpEnableSsl { get; }
    string SmtpHost { get; }
    string SmtpPassword { get; }
    int SmtpPort { get; }
    string SmtpUsername { get; }
    string B2AppKey { get; }
    string B2KeyId { get; }
    string B2BucketId { get; }
    string B2BucketName { get; }
}