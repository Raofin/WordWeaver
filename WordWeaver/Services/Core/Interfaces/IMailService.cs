using WordWeaver.Dtos;

namespace WordWeaver.Services.Core.Interfaces;

public interface IMailService
{
    Task<EmailDto> SendEmail(EmailDto email, long userId = 0, bool log = true);
}