using WordWeaver.Data;
using WordWeaver.Data.Entity;
using WordWeaver.Services.Core.Interfaces;

namespace WordWeaver.Services.Core;

public class LoggerService(IHttpContextAccessor httpContextAccessor, IAuthenticatedUser authenticatedUser, WordWeaverContext dbContext) : ILoggerService
{
    public async Task<string> Error(Exception exception, bool isHandled = true)
    {
        try
        {
            var httpContext = httpContextAccessor?.HttpContext;

            var errorLog = new Error {
                IsHandled = isHandled,
                Message = exception.Message,
                Exception = exception.GetType().FullName,
                RequestPath = httpContext?.Request?.Path,
                HttpMethod = httpContext?.Request?.Method,
                IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            };

            if (httpContext?.Request?.Headers != null && httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                errorLog.UserId = authenticatedUser?.UserIdNullable;
            }

            await dbContext.Errors.AddAsync(errorLog);
            await dbContext.SaveChangesAsync();

            return exception.Message;

        } catch (Exception)
        {
            return exception.Message;
        }
    }
}
