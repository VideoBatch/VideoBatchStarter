using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VideoBatch.Services
{
    /// <summary>
    /// Configuration options for the HTML Template Service.
    /// </summary>
    public class HtmlTemplateOptions
    {
        public const string Position = "HtmlTemplate";
        public string CssFilePath { get; set; } = "Resources/global.css"; // Default path relative to app root
    }

    /// <summary>
    /// Service for generating themed HTML documents using an external CSS file.
    /// </summary>
    public class HtmlTemplateService : IHtmlTemplateService
    {
        private readonly ILogger<HtmlTemplateService> _logger;
        private readonly HtmlTemplateOptions _options;
        private string _cssContent = string.Empty;
        private string _htmlTemplate = string.Empty;

        // Lock for thread-safe CSS loading
        private static readonly SemaphoreSlim _cssLoadLock = new SemaphoreSlim(1, 1);


        public HtmlTemplateService(IOptions<HtmlTemplateOptions> options, ILogger<HtmlTemplateService> logger)
        {
            _options = options.Value;
            _logger = logger;

            // Build the template structure once
            _htmlTemplate = BuildHtmlStructure();

            // Initial async load - don't block constructor
            _ = LoadCssTemplateAsync();
        }

        /// <inheritdoc />
        public async Task LoadCssTemplateAsync()
        {
            await _cssLoadLock.WaitAsync(); // Ensure only one thread loads at a time
            try
            {
                string cssFilePath = Path.GetFullPath(_options.CssFilePath); // Resolve to absolute path
                _logger.LogInformation("Attempting to load CSS from: {CssPath}", cssFilePath);

                if (!File.Exists(cssFilePath))
                {
                    _logger.LogError("CSS file not found at specified path: {CssPath}. Using empty styles.", cssFilePath);
                    _cssContent = "/* CSS File Not Found */";
                    return;
                }

                _cssContent = await File.ReadAllTextAsync(cssFilePath);
                _logger.LogInformation("Successfully loaded CSS template from {CssPath}. Length: {Length}", cssFilePath, _cssContent.Length);
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Failed to load CSS template from {CssPath}. Using empty styles.", _options.CssFilePath);
                _cssContent = "/* CSS Load Error */";
            }
            finally
            {
                _cssLoadLock.Release();
            }
        }

        /// <inheritdoc />
        public string BuildThemedHtml(string title, string bodyContent)
        {
            // Use the pre-built template and inject dynamic parts
            // Ensure thread safety when accessing _cssContent if LoadCssTemplateAsync could be called concurrently after initialization
            // However, typical DI scopes (Scoped/Singleton) and the initial load pattern make this less likely.
            // The lock in LoadCssTemplateAsync helps prevent race conditions during loading itself.
            string currentCss = _cssContent; // Read volatile field if needed, but string is immutable
            
            return string.Format(_htmlTemplate, 
                WebUtility.HtmlEncode(title), // Basic encoding for title
                currentCss,
                bodyContent // Assume bodyContent is already valid HTML (e.g., from Markdown conversion)
            );
        }

        /// <inheritdoc />
        public string BuildErrorHtml(string title, string errorMessage)
        {
            // Wrap the error message in a specific div/style for the CSS to target
            string errorBody = $"<h1>Error</h1><p class=\"error-message\">{WebUtility.HtmlEncode(errorMessage)}</p>";
            return BuildThemedHtml(title, errorBody);
        }

        /// <summary>
        /// Builds the basic HTML structure with placeholders for dynamic content.
        /// </summary>
        private string BuildHtmlStructure()
        {
            // Use indexed placeholders for string.Format
            return """
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>{0}</title>
                <style>
                {1}
                </style>
            </head>
            <body>
                {2}
            </body>
            </html>
            """;
        }
    }
} 