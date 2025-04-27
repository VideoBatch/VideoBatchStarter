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
using VideoBatch.Services; // Required for IDataService, IHtmlTemplateService

namespace VideoBatchApp // Use main application namespace
{
    /// <summary>
    /// Factory responsible for creating WorkArea instances for specific data models.
    /// </summary>
    public class WorkAreaFactory : IWorkAreaFactory
    {
        private readonly IDataService _dataService;
        private readonly IHtmlTemplateService _htmlTemplateService; // Added
        private readonly IServiceProvider _serviceProvider; // Keep for future use with DI resolution
        private readonly ILogger<WorkAreaFactory> _logger;
        private readonly MarkdownPipeline _markdownPipeline; // For Markdig

        public WorkAreaFactory(
            IDataService dataService, // Inject data service
            IHtmlTemplateService htmlTemplateService, // Inject template service
            IServiceProvider serviceProvider,
            ILogger<WorkAreaFactory> logger)
        {
            _dataService = dataService;
            _htmlTemplateService = htmlTemplateService;
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Configure Markdig Pipeline (can add extensions here if needed)
            _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

             _logger.LogInformation("WorkAreaFactory initialized.");
        }

        /// <inheritdoc />
        public async Task<WorkArea> CreateAsync(Guid id)
        {
             _logger.LogInformation("Attempting to create WorkArea for ID: {PrimitiveId}", id);
            Primitive? primitive = null;
            string errorMessage = null;
            string errorTitle = null;

            try
            {
                primitive = await _dataService.GetPrimitiveByIdAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                 _logger.LogError(ex, "Failed to get primitive for ID {PrimitiveId} because data service is not loaded.", id);
                 errorMessage = "Error creating document: Data service not loaded.";
                 errorTitle = $"Data service error for ID {id}";
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An unexpected error occurred retrieving primitive for ID {PrimitiveId}", id);
                 errorMessage = $"An unexpected error occurred while loading data for ID {id}.";
                 errorTitle = $"Loading error for ID {id}";
            }

            if (primitive == null && errorMessage == null) // Check if primitive is null *and* no prior error occurred
            {
                 _logger.LogWarning("Primitive with ID {PrimitiveId} not found by data service.", id);
                 errorMessage = $"Could not find data for ID: {id}";
                 errorTitle = $"Data Not Found: {id}";
            }

            // If an error occurred during data retrieval or primitive wasn't found, return an error WorkArea
            if (errorMessage != null)
            {
                 var errorWorkArea = new WorkArea(null); // Basic WorkArea
                 // Add the error HTML content directly to the WorkArea, 
                 // The UI layer is responsible for rendering this (e.g., in a WebView)
                 errorWorkArea.Tag = _htmlTemplateService.BuildErrorHtml(errorTitle ?? "Error", errorMessage);
                 errorWorkArea.DockText = errorTitle ?? "Error";
                 _logger.LogDebug("Returning Error WorkArea for ID: {PrimitiveId}, Title: {ErrorTitle}", id, errorTitle);
                 return errorWorkArea;
            }

             _logger.LogDebug("Found primitive: {PrimitiveName}, Type: {PrimitiveType}", primitive.Name, primitive.GetType().Name);

            // --- Create Specific WorkArea View ---
            // Currently only Markdown view is implemented
            WorkArea workArea;
            try
            {
                workArea = await CreateMarkdownViewWorkArea(primitive);
                 _logger.LogInformation("Successfully created Markdown view WorkArea for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to create Markdown WorkArea view for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
                 // Fallback to an error WorkArea if view creation fails
                 workArea = new WorkArea(primitive); // Basic fallback with primitive context if available
                 errorMessage = $"Error displaying data for {primitive.Name}.\n{ex.Message}";
                 errorTitle = $"Display Error: {primitive.Name}";
                 workArea.Tag = _htmlTemplateService.BuildErrorHtml(errorTitle, errorMessage);
                 workArea.DockText = errorTitle;
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
            var workArea = new WorkArea(primitive); // Constructor sets DockText based on primitive name
            var webView = new WebView2
            {
                Dock = DockStyle.Fill,
                DefaultBackgroundColor = System.Drawing.Color.FromArgb(31, 31, 31) // Match body background
            };
            workArea.Controls.Add(webView);

            // Generate Markdown content (Could be extracted to another service later)
            var markdownBuilder = new StringBuilder();
            markdownBuilder.AppendLine($"# {primitive.GetType().Name}: {primitive.Name}");
            markdownBuilder.AppendLine("```properties");
            markdownBuilder.AppendLine($"ID          : {primitive.ID}");
            markdownBuilder.AppendLine($"ParentID    : {primitive.ParentID}");
            markdownBuilder.AppendLine($"DateCreated : {primitive.DateCreated} (UTC)");
            markdownBuilder.AppendLine($"DateUpdated : {primitive.DateUpdated} (UTC)");
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

            // Create full HTML document using the template service
            var htmlContent = _htmlTemplateService.BuildThemedHtml(primitive.Name ?? "Details", htmlBody);

            _logger.LogDebug("HTML Content bytes to navigate: {Length}", Encoding.UTF8.GetBytes(htmlContent).Length); // Avoid logging potentially huge HTML string

            try
            {
                await webView.EnsureCoreWebView2Async(null);
                 _logger.LogDebug("CoreWebView2 initialized for {PrimitiveName}. Navigating to string content.", primitive.Name);
                webView.NavigateToString(htmlContent);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to initialize CoreWebView2 or navigate for {PrimitiveName}. Raising exception.", primitive.Name);
                 workArea.Controls.Remove(webView); // Clean up failed control
                 webView.Dispose();
                 // Throw an exception instead of trying to display error within this method
                 // The caller (CreateAsync) will catch this and create an error WorkArea.
                 throw new InvalidOperationException($"Failed to display details view for {primitive.Name}. WebView2 Error: {ex.Message}", ex);
            }

            return workArea;
        }

        // Removed BuildThemedHtml - responsibility moved to HtmlTemplateService

        // Removed AddErrorTextWebView - error handling simplified in CreateAsync

        // TODO: Implement Save handler later if needed
        // private void WorkAreaFactory_SaveDocument(object sender, EventArgs e)
        // {
        //    // Use _dataService to save changes from the WorkArea (sender)
        // }
    }
} 