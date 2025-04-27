using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms.Docking
{
    public class AssetsDock : ToolWindow
    {
        public AssetsDock()
        {
            InitializeComponent();
            DefaultDockArea = DockArea.Bottom;
            DockText = "Assets";
            Text = "Assets";
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
            // TODO: Consider renaming this method if its purpose changes with the "Assets" concept
        }
    }
} 