using AcrylicUI.Controls;
using AcrylicUI.Docking;
using System.Windows.Forms;
using System.Drawing; // Added for Point
using VideoBatch.Services; // Added for Queue Service
using System.Linq; // For DequeueAllLogs

namespace VideoBatch.UI.Forms.Docking
{
    public partial class OutputDock : ToolWindow // Added partial for Timer disposal
    {
        private AcrylicTextBox logTextBox;
        private AcrylicPanel scrollPanel; // Added Panel
        private readonly OutputLogQueueService _queueService;
        private System.Windows.Forms.Timer? _queueTimer;

        // Constructor now requires Queue Service
        public OutputDock(OutputLogQueueService queueService)
        {
            _queueService = queueService;

            InitializeComponent(); // Call private method to setup controls
            InitializeQueueTimer();
        }

        // Separate method to keep constructor clean
        private void InitializeComponent()
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

        private void InitializeQueueTimer()
        {
            _queueTimer = new System.Windows.Forms.Timer();
            _queueTimer.Interval = 250; // Check queue every 250ms
            _queueTimer.Tick += QueueTimer_Tick;
            _queueTimer.Start();
        }

        private void QueueTimer_Tick(object? sender, EventArgs e)
        {
            _queueTimer?.Stop(); 

            if (!_queueService.IsEmpty)
            {
                var messages = _queueService.DequeueAllLogs().ToList(); 

                if (messages.Any())
                {
                    AppendLog(string.Join(Environment.NewLine, messages));
                }
            }

            _queueTimer?.Start(); 
        }

        // Method to append log messages (thread-safe)
        // This is now called by the QueueTimer_Tick
        private void AppendLog(string message)
        {
            if (this.IsDisposed || logTextBox.IsDisposed) return; // Safety check

            if (logTextBox.InvokeRequired)
            {
                try
                {
                    logTextBox.Invoke(new Action<string>(AppendLog), message);
                }
                catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
                {                    
                    System.Diagnostics.Debug.WriteLine($"[OutputDock] Exception during Invoke (likely form closing): {ex.Message}");
                }
            }
            else
            {
                try
                {
                    var wasAtBottom = false;
                    // Check if scrollbar is at the bottom BEFORE appending
                    if (scrollPanel.VerticalScroll.Visible)
                    {
                        wasAtBottom = (scrollPanel.VerticalScroll.Value >= scrollPanel.VerticalScroll.Maximum - scrollPanel.VerticalScroll.LargeChange);
                    }
                    else
                    {
                        wasAtBottom = true; // No scrollbar means we are effectively at the bottom
                    }

                    logTextBox.AppendText(message + Environment.NewLine);

                    // Auto-scroll only if it was already at the bottom
                    if (wasAtBottom && scrollPanel.VerticalScroll.Visible)
                    {
                         // Scroll to the new maximum
                         scrollPanel.ScrollControlIntoView(logTextBox); // Often works better
                         // Alternative:
                         // scrollPanel.AutoScrollPosition = new Point(0, scrollPanel.VerticalScroll.Maximum - scrollPanel.ClientSize.Height); 
                         // scrollPanel.PerformLayout(); // Force layout update after scrolling
                    }
                }
                catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
                {                     
                    System.Diagnostics.Debug.WriteLine($"[OutputDock] Exception during direct execution (likely form closing): {ex.Message}");
                } 
            }
        }

        // Dispose the timer
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _queueTimer?.Stop();
                _queueTimer?.Dispose();
                _queueTimer = null;
            }
            base.Dispose(disposing);
        }
    }
} 