using System;
using System.Threading.Tasks;
using VideoBatch.Model; // Reference the model project

namespace VideoBatch.Services // Namespace reflects location
{
    /// <summary>
    /// Service interface for loading and accessing project data from a JSON source.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Loads the project data from the specified JSON file path and caches it.
        /// </summary>
        /// <param name="filePath">The path to the JSON data file.</param>
        /// <returns>The loaded Account object.</returns>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if the file specified by <paramref name="filePath"/> does not exist.</exception>
        /// <exception cref="System.Text.Json.JsonException">Thrown if the file contents are invalid JSON or cannot be deserialized into the Account model.</exception>
        Task<Account> LoadDataAsync(string filePath);

        /// <summary>
        /// Gets the cached root Account object. Requires LoadDataAsync to be called successfully first.
        /// </summary>
        /// <returns>The cached Account object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if data has not been loaded yet.</exception>
        Task<Account> GetAccountAsync();

        /// <summary>
        /// Searches the cached data for a Primitive object matching the specified ID.
        /// Requires LoadDataAsync to be called successfully first.
        /// </summary>
        /// <param name="id">The unique identifier of the Primitive to find.</param>
        /// <returns>The found Primitive object, or null if no object with the specified ID exists in the loaded data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if data has not been loaded yet.</exception>
        Task<Primitive?> GetPrimitiveByIdAsync(Guid id); // Return type made nullable
    }
} 