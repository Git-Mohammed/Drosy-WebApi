using Microsoft.Extensions.Logging;

namespace Drosy.Infrastructure.Logging
{
    public class LoggerAdapter<T>(Microsoft.Extensions.Logging.ILogger<T> logger) : Application.Interfaces.Common.ILogger<T>
    {
        private readonly Microsoft.Extensions.Logging.ILogger<T> _logger = logger;

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }
    }
}