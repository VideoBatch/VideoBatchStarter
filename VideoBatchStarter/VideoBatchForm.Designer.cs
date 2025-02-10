
using AcrylicUI.Controls;
using AcrylicUI.Resources;

namespace VideoBatch.UI.Forms
{
    partial class VideoBatchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            windowPanel1 = new AcrylicUI.Controls.WindowPanel();
            DockPanel = new AcrylicUI.Docking.DockPanel();
            menuStrip = new AcrylicUI.Controls.AcrylicMenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip = new AcrylicUI.Controls.AcrylicStatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusTimer = new System.Windows.Forms.Timer(components);
            statusLblVersion = new ToolStripStatusLabel();
            windowPanel1.SuspendLayout();
            statusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // windowPanel1
            // 
            windowPanel1.Controls.Add(DockPanel);
            windowPanel1.Controls.Add(menuStrip);
            windowPanel1.Controls.Add(statusStrip);
            windowPanel1.Dock = DockStyle.Fill;
            windowPanel1.Icon = null;
            windowPanel1.IsAcrylic = false;
            windowPanel1.Location = new Point(0, 0);
            windowPanel1.Margin = new Padding(0, 0,0, 0);
            windowPanel1.Name = "windowPanel1";
            windowPanel1.RoundCorners = false;
            windowPanel1.SectionHeader = null;
            windowPanel1.Size = new Size(800, 450);
            windowPanel1.TabIndex = 0;
            // 
            // DockPanel
            // 
            DockPanel.BackColor = Colors.MontereyDark;
            DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            DockPanel.Location = new Point(0, 35);
            DockPanel.Margin = new Padding(4, 5, 4, 5);
            DockPanel.Name = "DockPanel";
            DockPanel.Size = new System.Drawing.Size(1983, 1174);
            DockPanel.TabIndex = 0;
            //
            // menuStrip
            //
            menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            menuStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                fileToolStripMenuItem
            });
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "mnuMain";
            menuStrip.TabIndex = 1;
            menuStrip.Text = "acrylicMenuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            });
            fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // statusStrip
            // 
            statusStrip.AutoSize = false;
            statusStrip.BackColor = Colors.MontereyDark;
            statusStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
 statusLabel,statusLblVersion});
            statusStrip.Location = new Point(0, 1209);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(5, 0, 0, 0);
            statusStrip.Size = new System.Drawing.Size(1983, 30);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 0;
            statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = false;
            statusLabel.Margin = new Padding(0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(400, 41);
            statusLabel.Text = "Ready";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statusTimer
            // 
            statusTimer.Interval = 10000;
            // 
            // statusLblVersion
            // 
            statusLblVersion.AutoSize = false;
            statusLblVersion.Name = "statusLblVersion";
            statusLblVersion.Size = new Size(1886, 75);
            statusLblVersion.Spring = true;
            statusLblVersion.Text = "version:";
            statusLblVersion.TextAlign = ContentAlignment.MiddleRight;
            // 
            // VideoBatchForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(windowPanel1);
            Location = new Point(0, 0);
            MainMenuStrip = menuStrip;
            Name = "VideoBatchForm";
            Text = "Form5";
            windowPanel1.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private AcrylicUI.Controls.WindowPanel windowPanel1;
        private AcrylicUI.Docking.DockPanel DockPanel;
        private AcrylicUI.Controls.AcrylicMenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private AcrylicUI.Controls.AcrylicStatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Timer statusTimer;
        private ToolStripStatusLabel statusLblVersion;
    }
}