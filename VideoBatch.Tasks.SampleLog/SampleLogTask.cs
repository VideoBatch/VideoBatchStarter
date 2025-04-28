using System;
using System.Collections.Generic;
using System.Diagnostics; // For Debug.WriteLine
using System.IO; // Added for Path.Combine
using System.Linq; // Added for FirstOrDefault
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VideoBatch.Model;
using VideoBatch.Tasks.Interfaces;
using System.Threading; // Added for CancellationToken

namespace VideoBatch.Tasks.SampleLog
{
    public class SampleLogTask : IBatchTask
    {
        // Unique ID for this *type* of task. Generate a new GUID for each task type.
        // You can generate one in Visual Studio via Tools > Create GUID
        public Guid ID => new Guid("8D3D50DF-52B1-4550-8665-B4322D6757FE"); 
        
        public string Name => "Sample Log Task";

        public string Description => "Logs a configurable message and demonstrates input/output file path handling.";

        public string Version => "1.1";

        // Since we removed Category from the Interface, we remove it here too.
        // public string Category => "Debug";

        // Define the "Message" property
        private const string MessagePropertyName = "Message";

        public IEnumerable<TaskProperty> GetPropertyDefinitions()
        {
            yield return new TaskProperty
            {
                Name = MessagePropertyName,
                Description = "The text message to log.",
                Type = "string", // Indicate data type
                DefaultValue = "Default log message!", // Provide a default
                IsRequired = true // Let's make it required
            };
        }

        // The main execution logic for the task.
        public Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context, CancellationToken cancellationToken)
        {
            string messageToLog = "No message property provided!"; // Default if not found

            // Log received InputFilePath
            string inputPathMessage = $"InputFilePath received: {(string.IsNullOrEmpty(context.InputFilePath) ? "<None>" : context.InputFilePath)}";
            Debug.WriteLine($"[TASK LOG] {inputPathMessage}");
            context.Messages.Add(inputPathMessage);

            // Check for cancellation early
            if (cancellationToken.IsCancellationRequested)
            {
                context.Messages.Add("[CANCELLED] Task cancelled before execution started.");
                context.HasError = true; // Treat cancellation as error state
                return Task.FromResult(context);
            }

            // Try to get the message from the context properties
            if (context.Properties.TryGetValue(MessagePropertyName, out object? messageValue) && messageValue is string messageString)
            {
                messageToLog = messageString;
            }
            else
            {
                 // Optionally fall back to the default defined in GetPropertyDefinitions
                 var propDef = GetPropertyDefinitions().FirstOrDefault(p => p.Name == MessagePropertyName);
                 if (propDef?.DefaultValue is string defaultMsg)
                 {
                     messageToLog = defaultMsg + " (Used Default)";
                 }
                 context.Messages.Add($"Warning: Property '{MessagePropertyName}' not found or not a string in context. Using fallback/default.");
                 // Optionally set context.HasError = true if it's critical
            }

            // Log the retrieved or default message
            string logEntry = $"Executing {Name}: {messageToLog}";
            Debug.WriteLine($"[TASK LOG] {logEntry}");
            context.Messages.Add(logEntry);

            Debug.WriteLine("[TASK TRACE] Entering file creation block...");
            // --- Create Dummy Output File --- 
            string outputFilePath = Path.Combine(Path.GetTempPath(), $"SampleLogTaskOutput_{Guid.NewGuid()}.txt");
            try
            {
                // Check for cancellation before file operations
                cancellationToken.ThrowIfCancellationRequested();

                Debug.WriteLine("[TASK TRACE] Inside try block...");
                string fileContent = $"--- SampleLogTask Execution ---\n";
                fileContent += $"Timestamp: {DateTime.UtcNow:o}\n";
                fileContent += $"Input Path: {context.InputFilePath ?? "<None>"}\n";
                fileContent += $"Message Logged: {messageToLog}\n";
                fileContent += $"Properties Provided: \n";
                foreach(var prop in context.Properties)
                {
                    fileContent += $"  - {prop.Key}: {prop.Value}\n";
                }
                
                Debug.WriteLine($"[TASK TRACE] Attempting to write to: {outputFilePath}");
                File.WriteAllText(outputFilePath, fileContent);
                Debug.WriteLine("[TASK TRACE] File write successful.");
                
                context.OutputFilePath = outputFilePath; // Set the output path on the context
                string outputMessage = $"Created dummy output file: {outputFilePath}";
                 Debug.WriteLine($"[TASK LOG] {outputMessage}");
                context.Messages.Add(outputMessage);
            }
            catch(Exception ex)
            {
                string errorMessage = $"Error creating dummy output file ({ex.GetType().Name}): {ex.Message}";
                 Debug.WriteLine($"[TASK ERROR] {errorMessage}");
                 Debug.WriteLine($"[TASK ERROR] StackTrace: {ex.StackTrace}");
                 context.Messages.Add(errorMessage);
                 context.HasError = true; 
            }
            // --------------------------------
            Debug.WriteLine("[TASK TRACE] Exiting ExecuteAsync method...");

            return Task.FromResult(context);
        }
    }
}