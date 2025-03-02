using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms.Docking
{
    public class BatchProcessingDock : ToolWindow
    {
        public BatchProcessingDock()
        {
            InitializeComponent();
            DefaultDockArea = DockArea.Right;
            DockText = "Batch Processing";
            Text = "Batch Processing";
        }

        private void InitializeComponent()
        {
            // Will be implemented later with proper UI controls
            BackColor = Colors.GreyBackground;
        }

        public void ConfigureProcessingChain()
        {
            // Will be implemented later
            // This method will handle processing chain configuration
        }
    }
} 