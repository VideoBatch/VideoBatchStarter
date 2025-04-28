using Microsoft.Extensions.Logging;
using System;
using VideoBatch.Services; // Added for Queue Service
// using System.Diagnostics; // Removed for Debug.WriteLine

namespace VideoBatch.Logging
{
    public class OutputDockLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly OutputLogQueueService _queueService;

        // Constructor now requires Queue Service instead of Provider
        public OutputDockLogger(string categoryName, OutputLogQueueService queueService)
        {
            _categoryName = categoryName;
            _queueService = queueService;
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

            // Enqueue the log entry
            _queueService.EnqueueLog(logEntry);
        }
    }
} 