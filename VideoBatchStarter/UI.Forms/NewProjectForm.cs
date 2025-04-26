using System;
using System.Windows.Forms;
using AcrylicUI.Forms;
using AcrylicUI.Controls;
using AcrylicUI.Resources;
using NodaTime;
using System.ComponentModel;

namespace VideoBatch.UI.Forms
{
    public partial class NewProjectForm : AcrylicDialog
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string ProjectName { get; private set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public string ProjectLocation { get; private set; } = string.Empty;

        private AcrylicTextBox txtProjectName = new();
        private AcrylicTextBox txtLocation = new();
        private AcrylicButton btnBrowse = new();
        private AcrylicLabel lblProjectName = new();
        private AcrylicLabel lblLocation = new();

        public NewProjectForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void InitializeComponent()
        {
            txtProjectName = new AcrylicTextBox();
            txtLocation = new AcrylicTextBox();
            btnBrowse = new AcrylicButton();
            lblProjectName = new AcrylicLabel();
            lblLocation = new AcrylicLabel();
            SuspendLayout();
            // 
            // txtProjectName
            // 
            txtProjectName.BackColor = Color.FromArgb(31, 31, 31);
            txtProjectName.BorderStyle = BorderStyle.FixedSingle;
            txtProjectName.ForeColor = Color.FromArgb(245, 245, 245);
            txtProjectName.Location = new Point(332, 55);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.Size = new Size(728, 55);
            txtProjectName.TabIndex = 0;
            // 
            // txtLocation
            // 
            txtLocation.BackColor = Color.FromArgb(31, 31, 31);
            txtLocation.BorderStyle = BorderStyle.FixedSingle;
            txtLocation.ForeColor = Color.FromArgb(245, 245, 245);
            txtLocation.Location = new Point(332, 132);
            txtLocation.Name = "txtLocation";
            txtLocation.ReadOnly = true;
            txtLocation.Size = new Size(460, 55);
            txtLocation.TabIndex = 1;
            // 
            // btnBrowse
            // 
            btnBrowse.Default = false;
            btnBrowse.Image = null;
            btnBrowse.ImagePadding = 15;
            btnBrowse.Location = new Point(814, 131);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Padding = new Padding(5);
            btnBrowse.Size = new Size(246, 55);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = false;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblProjectName
            // 
            lblProjectName.AutoSize = true;
            lblProjectName.ForeColor = Color.FromArgb(192, 192, 192);
            lblProjectName.Location = new Point(67, 57);
            lblProjectName.Name = "lblProjectName";
            lblProjectName.Size = new Size(244, 48);
            lblProjectName.TabIndex = 6;
            lblProjectName.Text = "Project Name:";
            // 
            // lblLocation
            // 
            lblLocation.AutoSize = true;
            lblLocation.ForeColor = Color.FromArgb(192, 192, 192);
            lblLocation.Location = new Point(148, 139);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new Size(163, 48);
            lblLocation.TabIndex = 5;
            lblLocation.Text = "Location:";
            // 
            // NewProjectForm
            // 
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Colors.GreyBackground;
            ClientSize = new Size(1127, 345);
            Controls.Add(btnBrowse);
            Controls.Add(txtLocation);
            Controls.Add(lblLocation);
            Controls.Add(txtProjectName);
            Controls.Add(lblProjectName);
            DialogButtons = AcrylicDialogButton.OkCancel;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewProjectForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "New Project";
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupForm()
        {
            // Set default location to My Documents
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            txtLocation.Text = documentsPath;
            txtProjectName.Text = "VideoBatch01.json";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (var folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Select Project Location";
                folderBrowser.SelectedPath = documentsPath;
                folderBrowser.ShowNewFolderButton = true;

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    txtLocation.Text = folderBrowser.SelectedPath;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtProjectName.Text))
                {
                    MessageBox.Show("Please enter a project name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }

                ProjectName = txtProjectName.Text;
                ProjectLocation = txtLocation.Text;

                // Fix file naming: ensure only one .json extension
                if (!ProjectName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    ProjectName += ".json";
                }
            }
            base.OnFormClosing(e);
        }
    }
} 