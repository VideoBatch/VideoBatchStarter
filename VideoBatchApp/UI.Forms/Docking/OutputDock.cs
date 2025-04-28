using AcrylicUI.Controls;
using AcrylicUI.Docking;
using System.Windows.Forms;
using System.Drawing; // Added for Point

namespace VideoBatch.UI.Forms.Docking
{
    public class OutputDock : ToolWindow
    {
        private AcrylicTextBox logTextBox;
        private AcrylicPanel scrollPanel; // Added Panel

        public OutputDock()
        {
            // Configure the Panel for scrolling
            scrollPanel = new AcrylicPanel
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 25), // Offset for header
                Name = "scrollPanel",
                TabIndex = 1, // Ensure panel is behind textbox logically if needed, though docking handles layout
                AutoScroll = true // Enable scrolling on the panel
            };

            // Configure the TextBox (now inside the panel)
            logTextBox = new AcrylicTextBox
            {
                Dock = DockStyle.Fill, // Fill the panel
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.None, // Disable internal scrollbars
                BackColor = System.Drawing.Color.FromArgb(31, 31, 31),
                ForeColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                Name = "logTextBox",
                // Location is handled by Dock = Fill within the panel
                TabIndex = 0
            };

            // Add TextBox to the Panel
            scrollPanel.Controls.Add(logTextBox);

            // Add AutoScale settings to the form itself
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            // Add the Panel (which contains the TextBox) to the main control
            this.Controls.Add(scrollPanel);

            // Set Dock properties for the ToolWindow
            this.DefaultDockArea = DockArea.Bottom;
            this.DockText = "Output";
            this.Text = "Output";
        }

        // Method to append log messages (thread-safe)
        public void AppendLog(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action<string>(AppendLog), message);
            }
            else
            {
                // Append text
                logTextBox.AppendText(message + Environment.NewLine);

                // Auto-scroll the PANEL to the bottom
                // Requires panel to be focused or manual scrolling
                // A common trick is to set AutoScrollPosition after update
                scrollPanel.AutoScrollPosition = new Point(0, scrollPanel.VerticalScroll.Maximum);
                // Might need refinement based on how AcrylicPanel handles AutoScroll update
            }
        }
    }
} 