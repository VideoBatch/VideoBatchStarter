using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;
using System.Drawing;
using System.Windows.Forms;
using VideoBatch.Services;
using Microsoft.Extensions.Logging;
using VideoBatch.Tasks.Interfaces;
using VideoBatch.Model;
using System.Linq; // Added for LINQ methods
using System.Threading;
// using VideoBatch.UI.Controls; // Removed ProjectTree using

namespace VideoBatch.UI.Forms.Docking
{
    // Add partial keyword if a Designer.cs file exists for this class
    public partial class TaskExplorerDock : ToolWindow 
    {
        // Rename field
        private AcrylicTreeView taskTreeView; 
        private readonly ITaskDiscoveryService _taskDiscoveryService;
        // private readonly IProjectServices _projectServices; // <<< REMOVED
        // private readonly ProjectTree _projectTree; // <<< REMOVED
        private readonly ILogger<TaskExplorerDock> _logger;
        private IconFactory? _iconFactory;

        // Update constructor for DI
        public TaskExplorerDock(
            ITaskDiscoveryService taskDiscoveryService, 
            // IProjectServices projectServices, // <<< REMOVED
            // ProjectTree projectTree, // <<< REMOVED
            ILogger<TaskExplorerDock> logger)
        {
            // InitializeComponent(); // Remove this call
            _taskDiscoveryService = taskDiscoveryService;
            // _projectServices = projectServices; // <<< REMOVED
            // _projectTree = projectTree; // <<< REMOVED
            _logger = logger;

            // Create and configure TreeView programmatically
            this.taskTreeView = new AcrylicTreeView();
            this.SuspendLayout(); 

            this.taskTreeView.BackColor = System.Drawing.Color.FromArgb(31, 31, 31); 
            this.taskTreeView.DisableHorizontalScrollBar = false;
            this.taskTreeView.Dock = DockStyle.Fill;
            this.taskTreeView.Indent = 12;
            this.taskTreeView.ItemHeight = 22;
            this.taskTreeView.Location = new System.Drawing.Point(0, 25); 
            this.taskTreeView.MaxDragChange = 20;
            this.taskTreeView.Name = "taskTreeView"; // Rename control
            this.taskTreeView.TabIndex = 0;
            this.taskTreeView.Text = "taskTreeView";
            this.taskTreeView.ShowIcons = true;
           ;  

            // Add AutoScale settings here 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.Controls.Add(this.taskTreeView);
            this.ResumeLayout(false); 

            // Set Dock properties
            this.DefaultDockArea = DockArea.Right;
            this.DockText = "Task Explorer"; // Update DockText
            this.Text = "Task Explorer";

            InitializeContextMenu(); // Add this call back
        }

        // Remove the manual InitializeComponent method
        // private void InitializeComponent() { ... }

        // Add OnHandleCreated back
        protected override void OnHandleCreated(EventArgs e)
        {
             base.OnHandleCreated(e);
             _iconFactory = new IconFactory(IconFactory.GetDpiScale(Handle));
        }

        // Add OnLoad back
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PopulateTaskTree();
        }

        // Add PopulateTaskTree back
        private void PopulateTaskTree()
        {
            if (_iconFactory == null) {
                 _logger.LogWarning("IconFactory not initialized, cannot load task icons yet.");
                 if (IsHandleCreated) {
                     _iconFactory = new IconFactory(IconFactory.GetDpiScale(Handle));
                 }
                 if (_iconFactory == null) {
                      this.taskTreeView.Nodes.Add(new AcrylicTreeNode("Error: IconFactory unavailable."));
                      return; 
                 }                 
            }

            this.taskTreeView.Nodes.Clear();
            this.taskTreeView.SuspendLayout(); 

            try
            {
                var discoveredTasks = _taskDiscoveryService.DiscoveredTaskTypes;

                if (!discoveredTasks.Any())
                {
                    this.taskTreeView.Nodes.Add(new AcrylicTreeNode("No Tasks Found")); 
                    this.taskTreeView.ResumeLayout(true); 
                    return;
                }

                // Need to re-add Category to IJobTask first for this grouping to work!
                // For now, let's just list them without grouping.
                 _logger.LogInformation("Populating task tree with {Count} tasks (no category grouping yet).", discoveredTasks.Count());

                foreach (var taskType in discoveredTasks.OrderBy(t => t.Name))
                {
                    var taskInstance = _taskDiscoveryService.InstantiateTask(taskType);
                    if (taskInstance != null)
                    {
                         Image? taskIcon = _iconFactory?.BitmapFromSvg(AcrylicUI.Resources.Icons.Document_16x_svg);

                        AcrylicTreeNode taskNode = new AcrylicTreeNode(taskInstance.Name) 
                        {
                            Icon = taskIcon, 
                            Tag = taskInstance.GetType()
                        };
                        this.taskTreeView.Nodes.Add(taskNode);
                    }
                }

                // Original grouping code - requires Category on IJobTask
                /* 
                var groupedTasks = discoveredTasks
                    .Select(t => _taskDiscoveryService.InstantiateTask(t))
                    .Where(instance => instance != null) 
                    .GroupBy(instance => instance.Category ?? "Uncategorized") // Requires Category
                    .OrderBy(g => g.Key);

                foreach (var group in groupedTasks)
                {
                    Image? categoryIcon = _iconFactory?.BitmapFromSvg(AcrylicUI.Resources.Icons.Cube_16x_svg);
                    AcrylicTreeNode categoryNode = new AcrylicTreeNode(group.Key)
                    {
                         Icon = categoryIcon 
                    };
                    this.taskTreeView.Nodes.Add(categoryNode);

                    foreach (var taskInstance in group.OrderBy(t => t.Name))
                    {
                        Image? taskIcon = _iconFactory?.BitmapFromSvg(AcrylicUI.Resources.Icons.Document_16x_svg);
                        AcrylicTreeNode taskNode = new AcrylicTreeNode(taskInstance.Name) 
                        {
                            Icon = taskIcon, 
                            Tag = taskInstance.GetType()
                        };
                        categoryNode.Nodes.Add(taskNode);
                    }
                    // categoryNode.IsExpanded = true; // Doesn't work
                }
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating task tree.");
                this.taskTreeView.Nodes.Add(new AcrylicTreeNode("Error loading tasks."));
            }
            finally
            { 
                 this.taskTreeView.ResumeLayout(true); 
            }
        }

        // --- Context Menu - Adapted for AcrylicContextMenu ---

        // Change type and instantiate directly
        private readonly AcrylicContextMenu taskContextMenu = new AcrylicContextMenu();
        // Keep ToolStripMenuItems for easy reference if needed
        private ToolStripMenuItem? addItem;
        private ToolStripMenuItem? propsItem;
        private ToolStripMenuItem? refreshItem;

        private void InitializeContextMenu()
        {
            // Create standard ToolStripMenuItems
            addItem = new ToolStripMenuItem("Add to Workflow (NYI)");
            propsItem = new ToolStripMenuItem("Run Task (Debug)");
            refreshItem = new ToolStripMenuItem("Refresh Task List");

            // Add items to AcrylicContextMenu
            taskContextMenu.Items.AddRange(new ToolStripItem[] {
                 addItem, propsItem, new ToolStripSeparator(), refreshItem
             });

            // Wire up clicks on the ToolStripMenuItems
            addItem.Click += AddItem_Click;
            propsItem.Click += RunTaskMenuItem_Click;
            refreshItem.Click += RefreshItem_Click;

            // Keep MouseClick handler for showing the menu
            this.taskTreeView.MouseClick += TaskTreeView_MouseClick; 
        }

        private void TaskTreeView_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Determine selected node (might need adjustment if right-click doesn't select)
                var selectedNode = this.taskTreeView.SelectedNodes.FirstOrDefault(); 
                bool isTaskNodeSelected = selectedNode != null && selectedNode.Tag is Type;

                // Enable/disable items BEFORE showing
                if (addItem != null) addItem.Enabled = isTaskNodeSelected;
                if (propsItem != null) propsItem.Enabled = isTaskNodeSelected;
                // refreshItem is always enabled

                // Show the AcrylicContextMenu
                taskContextMenu.Show(this.taskTreeView, e.Location);
            }
        }

        private void AddItem_Click(object? sender, EventArgs e)
        {
            if (this.taskTreeView.SelectedNodes.FirstOrDefault()?.Tag is Type taskType)
            {
                 MessageBox.Show($"Placeholder: Add task '{taskType.Name}' to workflow.", "Not Yet Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void RunTaskMenuItem_Click(object? sender, EventArgs e)
        {
             if (this.taskTreeView.SelectedNodes.FirstOrDefault()?.Tag is Type taskType)
             {
                var taskInstance = _taskDiscoveryService.InstantiateTask(taskType);
                if (taskInstance != null) 
                {
                    // --- Add execution logic --- 
                    _logger.LogInformation("Attempting to execute task: {TaskName}", taskInstance.Name);
                    
                    // 1. Create a context
                    var context = new VideoBatchContext();

                    // Set a dummy InputFilePath for testing
                    context.InputFilePath = "C:\\path\\to\\dummy-input.tmp"; 

                    // Add a general note instead (Optional)
                    context.Messages.Add($"Note: Executing '{taskInstance.Name}' via debug runner. Using default/no properties.");
                    context.Messages.Add($"Note: Providing dummy InputFilePath: {context.InputFilePath}");

                    try
                    {
                        // 3. Execute the task
                        // Pass CancellationToken.None for now as this debug runner doesn't have cancellation UI
                        VideoBatchContext resultContext = await taskInstance.ExecuteAsync(context, CancellationToken.None); 
                        _logger.LogInformation("Task execution completed. Input: '{Input}', Output: '{Output}', HasError: {HasError}", 
                             context.InputFilePath, resultContext.OutputFilePath ?? "<None>", resultContext.HasError);

                        // 4. Display results (e.g., messages and OutputFilePath)
                        string resultMessage = $"Task '{taskInstance.Name}' executed."
                            + (resultContext.HasError ? " (With Error)" : "")
                            + $"\nInput Path Provided: {context.InputFilePath ?? "<None>"}"
                            + $"\nOutput Path Generated: {resultContext.OutputFilePath ?? "<None>"}"
                            + "\n\nMessages:\n" 
                            + string.Join("\n", resultContext.Messages);

                         MessageBox.Show(resultMessage, "Task Execution Result", MessageBoxButtons.OK, 
                            resultContext.HasError ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception during task execution: {TaskName}", taskInstance.Name);
                         MessageBox.Show($"Error executing task '{taskInstance.Name}':\n{ex.Message}", "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // --- End execution logic --- 
                }
                else
                {
                     MessageBox.Show($"Failed to instantiate task: {taskType.Name}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
             }
        }

        private void RefreshItem_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Refreshing task list...");
            _taskDiscoveryService.DiscoverTasks(); 
            PopulateTaskTree(); 
        }

        // Remove the placeholder method if not needed
        // public void ConfigureProcessingChain()
        // {
        //     // Will be implemented later
        // }

        // Ensure the private field is declared if InitializeComponent was removed entirely
        // private AcrylicTreeView taskTreeView; 
    }
} 