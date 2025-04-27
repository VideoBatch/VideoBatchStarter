using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoBatch.Model;

namespace VideoBatch.Tasks.Interfaces
{
    /// <summary>
    /// Defines the contract for a reusable task component within the VideoBatch system.
    /// </summary>
    public interface IJobTask
    {
        /// <summary>
        /// A unique identifier for the specific *type* of task.
        /// Implementations should typically return a constant Guid.
        /// Consistent with Primitive.ID
        /// </summary>
        Guid ID { get; } // Changed from Id to ID

        /// <summary>
        /// User-friendly name of the task.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Detailed description of what the task does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Category for organizing the task in the UI (e.g., "Audio", "Video", "Download").
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the definitions of the properties that this task accepts.
        /// </summary>
        /// <returns>An enumerable collection of TaskProperty definitions.</returns>
        IEnumerable<TaskProperty> GetPropertyDefinitions(); // Renamed from GetParameterDefinitions

        /// <summary>
        /// Executes the task's logic asynchronously.
        /// </summary>
        /// <param name="context">The execution context containing input data and properties.</param>
        /// <returns>A Task representing the asynchronous operation, resolving to the updated VideoBatchContext containing output data.</returns>
        Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context);
    }
}