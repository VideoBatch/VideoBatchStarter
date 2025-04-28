using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using VideoBatch.Services; // Added for Queue Service
// using VideoBatch.UI.Forms.Docking; // No longer needed here

namespace VideoBatch.Logging
{
    [ProviderAlias("OutputDock")]
    public class OutputDockLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, OutputDockLogger> _loggers = new();
        private readonly OutputLogQueueService _queueService;
        // private OutputDock? _outputDockInstance; // REMOVED

        // Constructor now requires the Queue Service
        public OutputDockLoggerProvider(OutputLogQueueService queueService)
        {
            _queueService = queueService;
        }

        // Property to hold the OutputDock instance - REMOVED
        // public OutputDock? OutputDockInstance
        // {
        //     get => _outputDockInstance;
        //     set => _outputDockInstance = value;
        // }

        // Create a logger instance, passing the Queue Service
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new OutputDockLogger(name, _queueService)); // Pass queue service
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
} 