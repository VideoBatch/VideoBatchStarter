using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms.Docking
{
    public class TaskExplorerDock : ToolWindow
    {
        public TaskExplorerDock()
        {
            InitializeComponent();
            DefaultDockArea = DockArea.Right;
            DockText = "Batch Processing";
            Text = "Batch Processing";
        }

        private void InitializeComponent()
        {
            acrylicTreeView1 = new AcrylicTreeView();
            SuspendLayout();
            // 
            // acrylicTreeView1
            // 
            acrylicTreeView1.BackColor = Color.FromArgb(31, 31, 31);
            acrylicTreeView1.DisableHorizontalScrollBar = false;
            acrylicTreeView1.Dock = DockStyle.Fill;
            acrylicTreeView1.Location = new Point(0, 24);
            acrylicTreeView1.MaxDragChange = 20;
            acrylicTreeView1.Name = "acrylicTreeView1";
            acrylicTreeView1.Size = new Size(312, 608);
            acrylicTreeView1.TabIndex = 0;
            acrylicTreeView1.Text = "acrylicTreeView1";
            // 
            // TaskExplorerDock
            // 
            BackColor = Color.FromArgb(31, 31, 31);
            Controls.Add(acrylicTreeView1);
            Name = "TaskExplorerDock";
            Size = new Size(312, 632);
            ResumeLayout(false);
        }

        public void ConfigureProcessingChain()
        {
            // Will be implemented later
            // This method will handle processing chain configuration
        }

        private AcrylicTreeView acrylicTreeView1;
    }
} 