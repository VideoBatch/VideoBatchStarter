using System.Threading.Tasks;

namespace VideoBatch.Services
{
    /// <summary>
    /// Defines a service for generating themed HTML documents.
    /// </summary>
    public interface IHtmlTemplateService
    {
        /// <summary>
        /// Builds a complete HTML document using the configured theme and the provided body content.
        /// </summary>
        /// <param name="title">The title for the HTML document.</param>
        /// <param name="bodyContent">The HTML content for the body.</param>
        /// <returns>A string containing the full HTML document.</returns>
        string BuildThemedHtml(string title, string bodyContent);

        /// <summary>
        /// Builds a themed HTML document specifically formatted to display an error message.
        /// </summary>
        /// <param name="title">The title for the error page.</param>
        /// <param name="errorMessage">The error message to display.</param>
        /// <returns>A string containing the full HTML document for the error.</returns>
        string BuildErrorHtml(string title, string errorMessage);

        /// <summary>
        /// Asynchronously loads or reloads the CSS template.
        /// Should be called during initialization and potentially if the CSS file can change.
        /// </summary>
        Task LoadCssTemplateAsync();
    }
} 