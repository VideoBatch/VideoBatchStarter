using System;

namespace VideoBatch.Model
{
    /// <summary>
    /// Defines a property that a JobTask can accept.
    /// </summary>
    public class TaskProperty
    {
        /// <summary>
        /// Unique identifier for the property within the task definition.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Data type of the property (e.g., string, int, bool, file).
        /// Using string for flexibility initially, could be an enum later.
        /// </summary>
        public string Type { get; set; } = "string";

        /// <summary>
        /// User-friendly description of the property.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Default value for the property if not provided by the user.
        /// Stored as object? Or string and parsed? Let's use object for now.
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Indicates if the property is required for the task to execute.
        /// </summary>
        public bool IsRequired { get; set; } = false;

        // Optional: Add validation rules, allowed values, etc. later
    }
} 