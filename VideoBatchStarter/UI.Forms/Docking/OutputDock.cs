using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms.Docking
{
    public class OutputDock : ToolWindow
    {
        public OutputDock()
        {
            InitializeComponent();
            DefaultDockArea = DockArea.Bottom;
            DockText = "Output";
            Text = "Output";
        }

        private void InitializeComponent()
        {
            // Will be implemented later with proper UI controls
            BackColor = Colors.GreyBackground;
        }

        public void LogProcessingStatus(string message)
        {
            // Will be implemented later
            // This method will display processing logs and results
        }
    }
} 