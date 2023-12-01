using System.Net;
using System.Net.Mail;
using WordWeaver.Data;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class MailService(IAppSettingsService appSettings, WordWeaverContext context) : IMailService
{
    public async Task<Email> SendEmail(Email email, long userId = 0, bool log = true)
    {
        try
        {
            var client = new SmtpClient(appSettings.SmtpHost, appSettings.SmtpPort)
            {
                EnableSsl = appSettings.SmtpEnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(appSettings.SmtpUsername, appSettings.SmtpPassword)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(appSettings.SmtpUsername),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email.To);

            if (email.CcRecipients != null)
            {
                foreach (var ccRecipient in email.CcRecipients)
                {
                    mailMessage.CC.Add(new MailAddress(ccRecipient));
                }
            }

            if (email.BccRecipients != null)
            {
                foreach (var bccRecipient in email.BccRecipients)
                {
                    mailMessage.Bcc.Add(new MailAddress(bccRecipient));
                }
            }

            await client.SendMailAsync(mailMessage);

            if (log)
            {
                context.Emails.Add(new Data.Entity.Email
                {
                    EmailTo = email.To,
                    Subject = email.Subject,
                    Body = email.Body,
                    CcRecipients = email.CcRecipients != null ? string.Join(", ", email.CcRecipients) : null,
                    BccRecipients = email.BccRecipients != null ? string.Join(", ", email.BccRecipients) : null,
                    CreatedBy = userId,
                });

                await context.SaveChangesAsync();
            }

            return email;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

public class Email
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public List<string>? CcRecipients { get; set; }
    public List<string>? BccRecipients { get; set; }
}