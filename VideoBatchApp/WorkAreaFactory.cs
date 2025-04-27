using Markdig;
using Microsoft.Extensions.DependencyInjection; // Needed?
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.WinForms; // Added for WebView2
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms; // Required for RichTextBox, Label etc.
using VideoBatch.Model;
using VideoBatch.UI.Controls; // Required for base WorkArea
using VideoBatch.Services; // Required for IDataService

namespace VideoBatchApp // Use main application namespace
{
    /// <summary>
    /// Factory responsible for creating WorkArea instances for specific data models.
    /// </summary>
    public class WorkAreaFactory : IWorkAreaFactory
    {
        private readonly IDataService _dataService;
        private readonly IServiceProvider _serviceProvider; // Keep for future use with DI resolution
        private readonly ILogger<WorkAreaFactory> _logger;
        private readonly JsonSerializerOptions _jsonOptions; // For formatting JSON output
        private readonly MarkdownPipeline _markdownPipeline; // For Markdig

        public WorkAreaFactory(
            IDataService dataService, // Inject data service
            IServiceProvider serviceProvider,
            ILogger<WorkAreaFactory> logger)
        {
            _dataService = dataService;
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Configure options for pretty-printing JSON
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Add other options like NodaTime if needed later
            };

            // Configure Markdig Pipeline (can add extensions here if needed)
            _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

             _logger.LogInformation("WorkAreaFactory initialized.");
        }

        /// <inheritdoc />
        public async Task<WorkArea> CreateAsync(Guid id)
        {
             _logger.LogInformation("Attempting to create WorkArea for ID: {PrimitiveId}", id);
            Primitive? primitive = null;
            try
            {
                // Use injected data service
                primitive = await _dataService.GetPrimitiveByIdAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                // Catch case where data hasn't been loaded in data service
                 _logger.LogError(ex, "Failed to get primitive for ID {PrimitiveId} because data service is not loaded.", id);
                 var errorWorkArea = new WorkArea(null); // Create basic WorkArea
                 AddErrorTextWebView(errorWorkArea, $"Error creating document: Data service not loaded.", $"Data service error for ID {id}");
                 errorWorkArea.DockText = $"Error";
                 return errorWorkArea;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An unexpected error occurred retrieving primitive for ID {PrimitiveId}", id);
                 var errorWorkArea = new WorkArea(null); // Create basic WorkArea
                 AddErrorTextWebView(errorWorkArea, $"An unexpected error occurred while loading data for ID {id}.", $"Loading error for ID {id}");
                 errorWorkArea.DockText = $"Error";
                 return errorWorkArea;
            }

            if (primitive == null)
            {
                 _logger.LogWarning("Primitive with ID {PrimitiveId} not found by data service.", id);
                 var notFoundWorkArea = new WorkArea(null); // Create basic WorkArea
                 AddErrorTextWebView(notFoundWorkArea, $"Could not find data for ID: {id}", $"Data Not Found: {id}");
                 notFoundWorkArea.DockText = $"Not Found";
                 return notFoundWorkArea;
            }

             _logger.LogDebug("Found primitive: {PrimitiveName}, Type: {PrimitiveType}", primitive.Name, primitive.GetType().Name);

            // --- Temporary JSON Dump Implementation --- 
            // In the future, this switch would resolve specific WorkArea types 
            // (e.g., ProjectWorkArea, JobWorkArea) via _serviceProvider

            WorkArea workArea;
            try
            {
                // Create the WorkArea using the new Markdown view method
                workArea = await CreateMarkdownViewWorkArea(primitive);
                 _logger.LogInformation("Successfully created Markdown view WorkArea for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to create Markdown WorkArea view for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
                 workArea = new WorkArea(primitive); // Basic fallback
                 AddErrorTextWebView(workArea, $"Error displaying data for {primitive.Name}.\n{ex.Message}", $"Display Error: {primitive.Name}");
                 workArea.DockText = $"Display Error";
            }
            
            // TODO: Attach save logic later if needed
            // workArea.SaveDocument += WorkAreaFactory_SaveDocument;

            return workArea;
        }

        /// <summary>
        /// Creates a WorkArea that displays the Primitive's details as Markdown rendered in a WebView2.
        /// </summary>
        private async Task<WorkArea> CreateMarkdownViewWorkArea(Primitive primitive)
        {
            var workArea = new WorkArea(primitive); // Constructor sets DockText
            var webView = new WebView2
            {
                Dock = DockStyle.Fill,
            };
            workArea.Controls.Add(webView);

            // Generate Markdown content
            var markdownBuilder = new StringBuilder();
            markdownBuilder.AppendLine($"# {primitive.GetType().Name}: {primitive.Name}");
            markdownBuilder.AppendLine("```properties"); // Using properties for simple key-value look
            markdownBuilder.AppendLine($"ID          : {primitive.ID}");
            markdownBuilder.AppendLine($"ParentID    : {primitive.ParentID}");
            markdownBuilder.AppendLine($"DateCreated : {primitive.DateCreated} (UTC)"); // Assuming NodaTime Instant
            markdownBuilder.AppendLine($"DateUpdated : {primitive.DateUpdated} (UTC)");
            // Add type-specific properties if needed
            if (primitive is Job job)
            {
                 markdownBuilder.AppendLine($"Asset Count : {(job.Assets?.Count ?? 0)}");
                 markdownBuilder.AppendLine($"Task Count  : {(job.Tasks?.Count ?? 0)}");
            }
             if (primitive is Project project)
            {
                 markdownBuilder.AppendLine($"Job Count   : {(project.Jobs?.Count ?? 0)}");
            }
            markdownBuilder.AppendLine("```");

            // Convert Markdown to HTML
            var htmlBody = Markdown.ToHtml(markdownBuilder.ToString(), _markdownPipeline);

            // Create full HTML document with dark theme CSS
            var htmlContent = BuildThemedHtml(primitive.Name ?? "Details", htmlBody);

            try
            {
                // Initialize WebView2 and navigate
                await webView.EnsureCoreWebView2Async(null);
                 _logger.LogDebug("CoreWebView2 initialized for {PrimitiveName}. Navigating to string content.", primitive.Name);
                webView.NavigateToString(htmlContent);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to initialize CoreWebView2 or navigate for {PrimitiveName}.", primitive.Name);
                 workArea.Controls.Remove(webView); // Remove broken control
                 webView.Dispose();
                 AddErrorTextWebView(workArea, $"Failed to display details view.\nWebView2 Error: {ex.Message}", $"Display Error: {primitive.Name}");
            }

            return workArea;
        }

        /// <summary>
        /// Helper to build a full HTML document with basic dark theme CSS.
        /// </summary>
        private string BuildThemedHtml(string title, string bodyContent)
        {
            return $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>{title}</title>
                <style>
                    body {{ font-family: Segoe UI, -apple-system, BlinkMacSystemFont, Roboto, Oxygen-Sans, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif; background-color: #1F1F1F; color: #DCDCDC; padding: 10px; }}
                    h1 {{ color: #9CDCFE; border-bottom: 1px solid #444; padding-bottom: 5px; margin-top: 0; }}
                    code {{ font-family: 'Cascadia Mono', Consolas, 'Courier New', monospace; background-color: #2A2A2A; padding: 0.2em 0.4em; border-radius: 3px; }}
                    pre > code {{ display: block; padding: 10px; background-color: #1A1A1A; border: 1px solid #333; border-radius: 4px; overflow-x: auto; }}
                    /* Add more styles as needed */
                </style>
            </head>
            <body>
                {bodyContent}
            </body>
            </html>
            """;
        }

        /// <summary>
        /// Helper to add error text to a WorkArea using WebView2 for consistency.
        /// </summary>
        private async void AddErrorTextWebView(WorkArea workArea, string userMessage, string title)
        {
            var webView = new WebView2
            {
                Dock = DockStyle.Fill,
            };
            workArea.Controls.Add(webView);

            string errorHtmlBody = $"<h1>Error</h1><p style='color: #FF8A8A;'>{System.Net.WebUtility.HtmlEncode(userMessage)}</p>";
            string errorHtml = BuildThemedHtml(title, errorHtmlBody);

            try
            {
                await webView.EnsureCoreWebView2Async(null);
                webView.NavigateToString(errorHtml);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to display error message in WebView2 for title: {ErrorTitle}", title);
                // Fallback to Label if WebView fails even for errors
                workArea.Controls.Remove(webView);
                webView.Dispose();
                var errorLabel = new Label
                {
                    Text = userMessage, // Show simpler message if WebView fails
                    Dock = DockStyle.Fill,
                    ForeColor = System.Drawing.Color.FromArgb(220, 80, 80),
                    BackColor = System.Drawing.Color.FromArgb(31, 31, 31),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                workArea.Controls.Add(errorLabel);
            }
        }

        // TODO: Implement Save handler later if needed
        // private void WorkAreaFactory_SaveDocument(object sender, EventArgs e)
        // {
        //    // Use _dataService to save changes from the WorkArea (sender)
        // }
    }
} 