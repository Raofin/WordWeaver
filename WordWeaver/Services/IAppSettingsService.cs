﻿namespace WordWeaver.Services;

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
}