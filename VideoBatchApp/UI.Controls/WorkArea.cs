using AcrylicUI.Docking;
using VideoBatch.Model; // Reference the model project
using System.ComponentModel;

namespace VideoBatch.UI.Controls
{
    /// <summary>
    /// Base class for dockable document windows representing Projects, Jobs, Tasks, etc.
    /// </summary>
    public class WorkArea : Document
    {
        /// <summary>
        /// Gets the underlying data model object (Project, Job, etc.) represented by this WorkArea.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        /// <summary>
        /// Gets the underlying data model object (Project, Job, etc.) represented by this WorkArea.
        /// </summary>
        public virtual Primitive? Data { get; protected set; }

        /// <summary>
        /// Protected constructor for WinForms designer.
        /// </summary>
        protected WorkArea()
        {
            // InitializeComponent(); // If using visual designer for base properties
        }

        /// <summary>
        /// Creates a new WorkArea associated with the given Primitive data.
        /// </summary>
        /// <param name="data">The data model object (Project, Job, etc.).</param>
        public WorkArea(Primitive? data)
        {
            Initialize(data);
        }

        /// <summary>
        /// Initializes the WorkArea with the provided data.
        /// Can be called by derived classes if they use the parameterless constructor.
        /// </summary>
        /// <param name="data">The data model object.</param>
        protected virtual void Initialize(Primitive? data)
        {
            Data = data;
            // Set the tab text. Handle null data case.
            DockText = data?.Name ?? "Document";

            // TODO: Set Icon based on data type later
            // Example:
            // if (data is Project) Icon = Resources.ProjectIcon;
            // else if (data is Job) Icon = Resources.JobIcon;
        }

        // TODO: Add SaveDocument event and IsDirty property later if needed for tracking changes
        // public event EventHandler SaveDocument;
        // public virtual bool IsDirty { get; protected set; }
        // protected virtual void OnSaveDocument(EventArgs e) => SaveDocument?.Invoke(this, e);

        // Optional: Override Close() method to handle IsDirty check like in Fulfilled
        // public override void Close()
        // {
        //    if (IsDirty) { /* Prompt user to save */ }
        //    base.Close();
        // }
    }
}