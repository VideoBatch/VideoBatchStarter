using Microsoft.Extensions.Logging;
using System;
using VideoBatch.UI.Forms.Docking; // Assuming OutputDock is here
// using System.Diagnostics; // Removed for Debug.WriteLine

namespace VideoBatch.Logging
{
    public class OutputDockLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly OutputDockLoggerProvider _provider;

        // Constructor requires provider and category name (will be updated later for queue service)
        public OutputDockLogger(string categoryName, OutputDockLoggerProvider provider)
        {
            _categoryName = categoryName;
            _provider = provider;
        }

        // This logger doesn't use scopes
        public IDisposable BeginScope<TState>(TState state) => default!;

        // Check if the log level is enabled (can be customized later)
        public bool IsEnabled(LogLevel logLevel)
        {
            // For now, log everything Information and above
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Format the message
            var message = formatter(state, exception);
            var logEntry = $"[{logLevel.ToString().ToUpper().Substring(0,4)}] [{_categoryName}] {message}";
            if (exception != null)
            {
                logEntry += $"{Environment.NewLine}Exception: {exception}"; // Simple exception logging
            }

            // TODO: Enqueue the logEntry to the Queue Service instead
            // _provider.OutputDockInstance.AppendLog(logEntry); // REMOVED
        }
    }
} 