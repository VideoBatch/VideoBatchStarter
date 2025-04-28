using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
// using VideoBatch.UI.Forms.Docking; // No longer needed here

namespace VideoBatch.Logging
{
    [ProviderAlias("OutputDock")]
    public class OutputDockLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, OutputDockLogger> _loggers = new();
        // private OutputDock? _outputDockInstance; // REMOVED

        // Property to hold the OutputDock instance - REMOVED
        // public OutputDock? OutputDockInstance
        // {
        //     get => _outputDockInstance;
        //     set => _outputDockInstance = value;
        // }

        // Create a logger instance (will be updated for queue service)
        public ILogger CreateLogger(string categoryName)
        {
            // Need to inject QueueService here later
            return _loggers.GetOrAdd(categoryName, name => new OutputDockLogger(name, this));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
} 