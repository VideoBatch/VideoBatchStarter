using System;
using System.Threading.Tasks;
using VideoBatch.UI.Controls; // Reference WorkArea base class

namespace VideoBatchApp // Use main application namespace
{
    /// <summary>
    /// Interface for a factory that creates WorkArea instances based on a data model ID.
    /// </summary>
    public interface IWorkAreaFactory
    {
        /// <summary>
        /// Asynchronously creates a WorkArea instance for the data model object with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the data model object (e.g., Project, Job ID).</param>
        /// <returns>A task representing the asynchronous operation, containing the created WorkArea instance.</returns>
        Task<WorkArea> CreateAsync(Guid id);
    }
} 