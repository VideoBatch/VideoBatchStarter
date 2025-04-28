using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using VideoBatch.UI.Forms.Docking; // Assuming OutputDock is here

namespace VideoBatch.Logging
{
    [ProviderAlias("OutputDock")] // Alias for configuration
    public class OutputDockLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, OutputDockLogger> _loggers = new();
        private OutputDock? _outputDockInstance;

        // Property to hold the OutputDock instance (set via DI or manually)
        public OutputDock? OutputDockInstance
        {
            get => _outputDockInstance;
            set => _outputDockInstance = value;
        }

        // Create a logger instance for a specific category
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new OutputDockLogger(name, this));
        }

        // Dispose method (required by ILoggerProvider)
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
} 