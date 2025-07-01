namespace Drosy.Application.Interfaces.Common
{
    public interface ILogger<T>
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogCritical(string message, params object[] args);
    }
}