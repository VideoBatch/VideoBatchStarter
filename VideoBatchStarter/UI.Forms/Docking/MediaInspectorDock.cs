using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms.Docking
{
    public class MediaInspectorDock : ToolWindow
    {
        public MediaInspectorDock()
        {
            InitializeComponent();
            DefaultDockArea = DockArea.Bottom;
            DockText = "Media Inspector";
            Text = "Media Inspector";
        }

        private void InitializeComponent()
        {
            // Will be implemented later with proper UI controls
            BackColor = Colors.GreyBackground;
        }

        public void UpdateVideoDetails(string filePath)
        {
            // Will be implemented later
            // This method will display video metadata and properties
        }
    }
} 