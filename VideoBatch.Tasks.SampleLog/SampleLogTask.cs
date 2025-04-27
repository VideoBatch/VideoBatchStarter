using System;
using System.Collections.Generic;
using System.Diagnostics; // For Debug.WriteLine
using System.Threading.Tasks;
using VideoBatch.Model;
using VideoBatch.Tasks.Interfaces;

namespace VideoBatch.Tasks.SampleLog
{
    public class SampleLogTask : IJobTask
    {
        // Unique ID for this *type* of task. Generate a new GUID for each task type.
        // You can generate one in Visual Studio via Tools > Create GUID
        public Guid ID => new Guid("3C9BED24-7A7D-46B1-A6D2-968B052D1994"); // Example GUID - VS > Tools > Create GUID

        public string Name => "Sample Log Task";

        public string Description => "Logs a message to the debug output window.";

        // Since we removed Category from the Interface, we remove it here too.
        // public string Category => "Debug";

        // This task currently takes no properties.
        public IEnumerable<TaskProperty> GetPropertyDefinitions()
        {
            // Return an empty list or yield break
            yield break;
            // Or: return Enumerable.Empty<TaskProperty>();
            // Or: return new List<TaskProperty>();
        }

        // The main execution logic for the task.
        public Task<VideoBatchContext> ExecuteAsync(VideoBatchContext context)
        {
            // Log a simple message to the Debug output
            string message = $"Executing {Name} (ID: {ID})";
            Debug.WriteLine($"[TASK LOG] {message}");

            // Add message to the context for potential display later
            context.Messages.Add(message);

            // Since this task doesn't modify files or produce output files,
            // we just return the context as is.
            // We also don't set context.HasError = true unless something goes wrong.

            // Use Task.FromResult for simple synchronous operations wrapped in a Task
            return Task.FromResult(context);
        }
    }
}