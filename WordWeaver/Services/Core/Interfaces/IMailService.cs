namespace WordWeaver.Services.Core.Interfaces;

public interface IMailService
{
    Task<Email> SendEmail(Email email, long userId = 0, bool log = true);
}