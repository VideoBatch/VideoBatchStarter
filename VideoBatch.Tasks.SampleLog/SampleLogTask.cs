using System;
using System.Collections.Generic;
using System.Diagnostics; // For Debug.WriteLine
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VideoBatch.Model;
using VideoBatch.Tasks.Interfaces;

namespace VideoBatch.Tasks.SampleLog
{
    public class SampleLogTask : IJobTask
    {
        // Unique ID for this *type* of task. Generate a new GUID for each task type.
        // You can generate one in Visual Studio via Tools > Create GUID
        public Guid ID => new Guid("8D3D50DF-52B1-4550-8665-B4322D6757FE"); 
        
        public string Name => "Sample Log Task";

        public string Description => "Logs a configurable message to the debug output window.";

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
        public Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context)
        {
            string messageToLog = "No message property provided!"; // Default if not found

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

            return Task.FromResult(context);
        }
    }
}