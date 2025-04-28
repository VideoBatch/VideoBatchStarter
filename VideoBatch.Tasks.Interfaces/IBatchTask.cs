using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VideoBatch.Model;

namespace VideoBatch.Tasks.Interfaces
{
    /// <summary>
    /// Defines the contract for a task that can be discovered, configured,
    /// and executed by the VideoBatch system.
    /// </summary>
    public interface IBatchTask
    {
        /// <summary>
        /// Gets the unique identifier for this *type* of task.
        /// Implementations should typically return a constant Guid.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Gets the user-friendly name of the task.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a brief description of what the task does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the version of the task implementation.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the definitions of the properties that this task accepts for configuration.
        /// </summary>
        /// <returns>An enumerable collection of TaskProperty definitions. Return empty if not configurable.</returns>
        IEnumerable<TaskProperty> GetPropertyDefinitions();

        /// <summary>
        /// Executes the task asynchronously.
        /// </summary>
        /// <param name="context">The shared context object containing input/output paths, messages, and properties.</param>
        /// <param name="cancellationToken">A token to signal cancellation of the task.</param>
        /// <returns>
        /// A Task representing the asynchronous operation, yielding the updated VideoBatchContext.
        /// The context's 'HasError' property should be set to true if the task fails.
        /// The context's 'OutputFilePath' should be set if the task produces a primary file output.
        /// </returns>
        Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context, CancellationToken cancellationToken);
    }
} 