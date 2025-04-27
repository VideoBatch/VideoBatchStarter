using System.Collections.Generic;

namespace VideoBatch.Services
{
    /// <summary>
    /// Service to manage recently opened project files.
    /// </summary>
    public interface IRecentFilesService
    {
        /// <summary>
        /// Gets the list of recently opened file paths.
        /// </summary>
        /// <returns>A list of file paths.</returns>
        IList<string> GetRecentFiles();

        /// <summary>
        /// Adds a file path to the top of the recent files list.
        /// If the path already exists, it's moved to the top.
        /// The list is trimmed to a maximum size.
        /// </summary>
        /// <param name="path">The file path to add.</param>
        void AddRecentFile(string path);
    }
} 