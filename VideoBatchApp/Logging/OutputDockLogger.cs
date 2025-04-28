using Microsoft.Extensions.Logging;
using System;
using VideoBatch.UI.Forms.Docking; // Assuming OutputDock is here

namespace VideoBatch.Logging
{
    public class OutputDockLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly OutputDockLoggerProvider _provider;

        // Constructor requires provider and category name
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

            // Ensure we have the OutputDock instance
            if (_provider.OutputDockInstance == null)
            {
                // Optionally log to debug console if the dock isn't ready
                // System.Diagnostics.Debug.WriteLine("OutputDockLogger: OutputDock instance not available.");
                return;
            }

            // Format the message
            var message = formatter(state, exception);
            var logEntry = $"[{logLevel.ToString().ToUpper().Substring(0,4)}] [{_categoryName}] {message}";
            if (exception != null)
            {
                logEntry += $"{Environment.NewLine}Exception: {exception}"; // Simple exception logging
            }

            // Send the formatted message to the OutputDock's AppendLog method
            // AppendLog is already thread-safe
            _provider.OutputDockInstance.AppendLog(logEntry);
        }
    }
} 