using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VideoBatch.Services
{
    /// <summary>
    /// A thread-safe singleton service to queue log messages for later display.
    /// </summary>
    public class OutputLogQueueService
    {
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// Adds a log message to the queue.
        /// </summary>
        /// <param name="message">The log message to enqueue.</param>
        public void EnqueueLog(string message)
        {
            _logQueue.Enqueue(message);
        }

        /// <summary>
        /// Attempts to dequeue a single log message from the queue.
        /// </summary>
        /// <param name="message">The dequeued message, or null if the queue is empty.</param>
        /// <returns>True if a message was successfully dequeued, false otherwise.</returns>
        public bool TryDequeueLog(out string? message)
        {
            return _logQueue.TryDequeue(out message);
        }

        /// <summary>
        /// Dequeues all available log messages from the queue.
        /// </summary>
        /// <returns>An enumerable collection of log messages.</returns>
        public IEnumerable<string> DequeueAllLogs()
        {
            while (_logQueue.TryDequeue(out string? message))
            {
                yield return message;
            }
        }

        /// <summary>
        /// Gets the current number of messages in the queue.
        /// </summary>
        public int Count => _logQueue.Count;

        /// <summary>
        /// Checks if the queue is empty.
        /// </summary>
        public bool IsEmpty => _logQueue.IsEmpty;
    }
} 