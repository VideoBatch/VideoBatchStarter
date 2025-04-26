using System;
using System.Windows.Forms;
using AcrylicUI.Forms;
using AcrylicUI.Controls;
using AcrylicUI.Resources;
using NodaTime;
using System.ComponentModel;

namespace VideoBatch.UI.Forms
{
    public partial class NewProjectForm : AcrylicForm
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
        private AcrylicButton btnCreate = new();
        private AcrylicButton btnCancel = new();
        private AcrylicLabel lblProjectName = new();
        private AcrylicLabel lblLocation = new();

        public NewProjectForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void InitializeComponent()
        {
            this.txtProjectName = new AcrylicTextBox();
            this.txtLocation = new AcrylicTextBox();
            this.btnBrowse = new AcrylicButton();
            this.btnCreate = new AcrylicButton();
            this.btnCancel = new AcrylicButton();
            this.lblProjectName = new AcrylicLabel();
            this.lblLocation = new AcrylicLabel();
            this.SuspendLayout();

            // lblProjectName
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(12, 15);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(74, 13);
            this.lblProjectName.Text = "Project Name:";

            // txtProjectName
            this.txtProjectName.Location = new System.Drawing.Point(12, 31);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(360, 20);
            this.txtProjectName.TabIndex = 0;

            // lblLocation
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(12, 64);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(51, 13);
            this.lblLocation.Text = "Location:";

            // txtLocation
            this.txtLocation.Location = new System.Drawing.Point(12, 80);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(279, 20);
            this.txtLocation.TabIndex = 1;
            this.txtLocation.ReadOnly = true;

            // btnBrowse
            this.btnBrowse.Location = new System.Drawing.Point(297, 78);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Click += new EventHandler(btnBrowse_Click);

            // btnCreate
            this.btnCreate.Location = new System.Drawing.Point(216, 120);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "Create";
            this.btnCreate.Click += new EventHandler(btnCreate_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(297, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler(btnCancel_Click);

            // NewProjectForm
            this.AcceptButton = this.btnCreate;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.txtProjectName);
            this.Controls.Add(this.lblProjectName);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "New Project";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupForm()
        {
            // Set default location to My Documents
            txtLocation.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Select Project Location";
                folderBrowser.SelectedPath = txtLocation.Text;
                folderBrowser.ShowNewFolderButton = true;

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    txtLocation.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProjectName.Text))
            {
                MessageBox.Show("Please enter a project name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProjectName = txtProjectName.Text;
            ProjectLocation = txtLocation.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
} 