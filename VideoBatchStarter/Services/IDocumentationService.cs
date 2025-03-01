namespace VideoBatch.Services
{
    /// <summary>
    /// Provides functionality for displaying application documentation
    /// </summary>
    public interface IDocumentationService
    {
        /// <summary>
        /// Shows the application documentation in the default browser
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ShowDocumentationAsync();
    }
} 