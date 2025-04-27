
using AcrylicUI.Controls;

namespace VideoBatch.UI.Forms
{
    partial class RenameForm
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
            txtName = new ();
            lblNewName = new ();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(12, 12);
            // 
            // btnClose
            // 
            btnClose.Location = new Point(12, 12);
            // 
            // btnYes
            // 
            btnYes.Location = new Point(12, 12);
            // 
            // btnNo
            // 
            btnNo.Location = new Point(12, 12);
            // 
            // btnRetry
            // 
            btnRetry.Location = new Point(452, 12);
            // 
            // btnIgnore
            // 
            btnIgnore.Location = new Point(452, 12);
            // 
            // txtName
            // 
            txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            txtName.Location = new Point(89, 23);
            txtName.MaxLength = 35;
            txtName.Name = "txtName";
            txtName.Size = new Size(191, 23);
            txtName.TabIndex = 0;
            // 
            // lblNewName
            // 
            lblNewName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            lblNewName.AutoSize = true;
            lblNewName.Location = new Point(16, 25);
            lblNewName.Name = "lblNewName";
            lblNewName.Size = new System.Drawing.Size(72, 15);
            lblNewName.TabIndex = 2;
            lblNewName.Text = "New Name :";
            // 
            // FrmRename
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(292, 111);
            Controls.Add(lblNewName);
            Controls.Add(txtName);
            DoubleBuffered = true;
            //FlatBorder = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "FrmRename";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "FrmRename";
            Load += new System.EventHandler(RenameForm_Load);
            Controls.SetChildIndex(txtName, 0);
            Controls.SetChildIndex(lblNewName, 0);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private AcrylicTextBox txtName;
        private AcrylicTitle lblNewName;
    }
}