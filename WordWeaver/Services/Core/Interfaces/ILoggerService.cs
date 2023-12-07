namespace WordWeaver.Services.Core.Interfaces
{
    public interface ILoggerService
    {
        Task<string> Error(Exception exception, bool isHandled = true);
    }
}