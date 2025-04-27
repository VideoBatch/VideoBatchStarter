using Microsoft.Extensions.DependencyInjection; // Needed?
using Microsoft.Extensions.Logging;
using System;
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
                 AddErrorText(errorWorkArea, $"Error creating document: {ex.Message}");
                 errorWorkArea.DockText = $"Error";
                 return errorWorkArea;
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An unexpected error occurred retrieving primitive for ID {PrimitiveId}", id);
                 var errorWorkArea = new WorkArea(null); // Create basic WorkArea
                 AddErrorText(errorWorkArea, $"An unexpected error occurred while loading data for ID {id}.");
                 errorWorkArea.DockText = $"Error";
                 return errorWorkArea;
            }

            if (primitive == null)
            {
                 _logger.LogWarning("Primitive with ID {PrimitiveId} not found by data service.", id);
                 var notFoundWorkArea = new WorkArea(null); // Create basic WorkArea
                 AddErrorText(notFoundWorkArea, $"Could not find data for ID: {id}");
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
                switch (primitive)
                {
                    case Project p:
                         _logger.LogDebug("Creating temporary JSON view for Project: {ProjectName}", p.Name);
                        workArea = CreateTemporaryJsonDumpWorkArea(p);
                        break;
                    case Job j:
                         _logger.LogDebug("Creating temporary JSON view for Job: {JobName}", j.Name);
                        workArea = CreateTemporaryJsonDumpWorkArea(j);
                        break;
                    case JobTask jt:
                         _logger.LogDebug("Creating temporary JSON view for JobTask: {JobTaskName}", jt.Name);
                        workArea = CreateTemporaryJsonDumpWorkArea(jt);
                        break;
                    // Add cases for Team, Account if they should be openable
                    default:
                         _logger.LogWarning("Creating generic temporary JSON view for unknown Primitive type: {PrimitiveType}", primitive.GetType().Name);
                        workArea = CreateTemporaryJsonDumpWorkArea(primitive);
                        break;
                }
                 _logger.LogInformation("Successfully created temporary WorkArea for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to create temporary WorkArea view for {PrimitiveName} ({PrimitiveId})", primitive.Name, id);
                 workArea = new WorkArea(primitive); // Basic fallback
                 AddErrorText(workArea, $"Error displaying data for {primitive.Name}.\n{ex.Message}");
                 workArea.DockText = $"Display Error";
            }
            
            // TODO: Attach save logic later if needed
            // workArea.SaveDocument += WorkAreaFactory_SaveDocument;

            return workArea;
        }

        /// <summary>
        /// Helper method to create a simple WorkArea that displays the Primitive as JSON.
        /// </summary>
        private WorkArea CreateTemporaryJsonDumpWorkArea(Primitive primitive)
        {
            var tempWorkArea = new WorkArea(primitive); // Use base WorkArea, constructor sets DockText
            try
            {
                var jsonText = JsonSerializer.Serialize(primitive, _jsonOptions);
                var textBox = new RichTextBox // Use RichTextBox for potential formatting
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Text = jsonText,
                    Font = new System.Drawing.Font("Consolas", 9.75F), // Use a monospaced font
                    WordWrap = false, // Disable word wrap for better JSON readability
                    ScrollBars = RichTextBoxScrollBars.ForcedBoth // Ensure scrollbars are always visible
                };
                tempWorkArea.Controls.Add(textBox);
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to serialize primitive {PrimitiveName} to JSON for display.", primitive.Name);
                 AddErrorText(tempWorkArea, $"Failed to display data as JSON.\n{ex.Message}");
            }
            return tempWorkArea;
        }

        /// <summary>
        /// Helper to add error text to a WorkArea.
        /// </summary>
        private void AddErrorText(WorkArea workArea, string errorMessage)
        {
             var errorLabel = new Label
             {
                 Text = errorMessage,
                 Dock = DockStyle.Fill,
                 TextAlign = System.Drawing.ContentAlignment.MiddleCenter
             };
             workArea.Controls.Add(errorLabel);
        }

        // TODO: Implement Save handler later if needed
        // private void WorkAreaFactory_SaveDocument(object sender, EventArgs e)
        // {
        //    // Use _dataService to save changes from the WorkArea (sender)
        // }
    }
} 