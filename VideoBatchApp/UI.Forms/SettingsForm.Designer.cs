using System;

namespace VideoBatch.UI.Forms
{
    partial class SettingsForm
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
            lblSettings = new AcrylicUI.Controls.AcrylicLabel();
            linkLabel1 = new LinkLabel();
            AcrylicLabel2 = new AcrylicUI.Controls.AcrylicLabel();
            btnApplyAdAuth = new AcrylicUI.Controls.AcrylicButton();
            btnLogin = new AcrylicUI.Controls.AcrylicButton();
            lblTenantID = new AcrylicUI.Controls.AcrylicLabel();
            lblUsername = new AcrylicUI.Controls.AcrylicTitle();
            txtTenantID = new AcrylicUI.Controls.AcrylicTextBox();
            lblClientID = new AcrylicUI.Controls.AcrylicLabel();
            txtClientID = new AcrylicUI.Controls.AcrylicTextBox();
            lblAccountDetails = new AcrylicUI.Controls.AcrylicTitle();
            lblAccountID = new AcrylicUI.Controls.AcrylicLabel();
            txtAccountID = new AcrylicUI.Controls.AcrylicTextBox();
            txtAccountName = new AcrylicUI.Controls.AcrylicTextBox();
            lblAccountName = new AcrylicUI.Controls.AcrylicLabel();
            AcrylicLabel1 = new AcrylicUI.Controls.AcrylicLabel();
            AcrylicTitle1 = new AcrylicUI.Controls.AcrylicTitle();
            lblCreateUpdate = new AcrylicUI.Controls.AcrylicLabel();
            lblMessage = new AcrylicUI.Controls.AcrylicLabel();
            SuspendLayout();
            // 
            // btnOk
            // 
            btnOk.Default = true;
            btnOk.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(12, 12);
            btnCancel.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnClose
            // 
            btnClose.Location = new Point(12, 12);
            btnClose.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnYes
            // 
            btnYes.Location = new Point(12, 12);
            btnYes.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnNo
            // 
            btnNo.Location = new Point(12, 12);
            btnNo.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnRetry
            // 
            btnRetry.Location = new Point(452, 12);
            btnRetry.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnIgnore
            // 
            btnIgnore.Location = new Point(452, 12);
            btnIgnore.Padding = new Padding(5, 4, 5, 4);
            // 
            // btnApply
            // 
            btnApply.Location = new Point(12, 12);
            // 
            // lblSettings
            // 
            lblSettings.AutoSize = true;
            lblSettings.ForeColor = Color.FromArgb(220, 220, 220);
            lblSettings.Location = new Point(16, 20);
            lblSettings.Margin = new Padding(2, 0, 2, 0);
            lblSettings.Name = "lblSettings";
            lblSettings.Size = new Size(147, 48);
            lblSettings.TabIndex = 2;
            lblSettings.Text = "Settings";
            // 
            // linkLabel1
            // 
            linkLabel1.LinkColor = Color.Crimson;
            linkLabel1.Location = new Point(1207, 980);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(201, 92);
            linkLabel1.TabIndex = 15;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Upgrade";
            linkLabel1.TextAlign = ContentAlignment.MiddleCenter;
            linkLabel1.VisitedLinkColor = Color.FromArgb(192, 0, 0);
            // 
            // AcrylicLabel2
            // 
            AcrylicLabel2.AutoSize = true;
            AcrylicLabel2.ForeColor = Color.FromArgb(220, 220, 220);
            AcrylicLabel2.Location = new Point(437, 1002);
            AcrylicLabel2.Margin = new Padding(2, 0, 2, 0);
            AcrylicLabel2.Name = "AcrylicLabel2";
            AcrylicLabel2.Size = new Size(780, 48);
            AcrylicLabel2.TabIndex = 14;
            AcrylicLabel2.Text = "Upgrade to Premium for Azure AD sync support";
            // 
            // btnApplyAdAuth
            // 
            btnApplyAdAuth.Default = false;
            btnApplyAdAuth.Image = null;
            btnApplyAdAuth.ImagePadding = 15;
            btnApplyAdAuth.Location = new Point(417, 695);
            btnApplyAdAuth.Name = "btnApplyAdAuth";
            btnApplyAdAuth.Padding = new Padding(5);
            btnApplyAdAuth.Size = new Size(794, 63);
            btnApplyAdAuth.TabIndex = 9;
            btnApplyAdAuth.Text = "Register Azure AD";
            btnApplyAdAuth.UseVisualStyleBackColor = false;
            // 
            // btnLogin
            // 
            btnLogin.Default = false;
            btnLogin.Image = null;
            btnLogin.ImagePadding = 15;
            btnLogin.Location = new Point(417, 835);
            btnLogin.Name = "btnLogin";
            btnLogin.Padding = new Padding(5);
            btnLogin.Size = new Size(794, 59);
            btnLogin.TabIndex = 7;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            // 
            // lblTenantID
            // 
            lblTenantID.AutoSize = true;
            lblTenantID.ForeColor = Color.FromArgb(220, 220, 220);
            lblTenantID.Location = new Point(103, 620);
            lblTenantID.Margin = new Padding(2, 0, 2, 0);
            lblTenantID.Name = "lblTenantID";
            lblTenantID.Size = new Size(177, 48);
            lblTenantID.TabIndex = 6;
            lblTenantID.Text = "Tenant ID:";
            // 
            // lblUsername
            // 
            lblUsername.ForeColor = Color.FromArgb(245, 245, 245);
            lblUsername.Location = new Point(103, 775);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(1114, 59);
            lblUsername.TabIndex = 8;
            lblUsername.Text = "Not Logged In";
            // 
            // txtTenantID
            // 
            txtTenantID.BackColor = Color.FromArgb(69, 73, 74);
            txtTenantID.BorderStyle = BorderStyle.FixedSingle;
            txtTenantID.ForeColor = Color.FromArgb(220, 220, 220);
            txtTenantID.Location = new Point(417, 615);
            txtTenantID.Margin = new Padding(2);
            txtTenantID.Name = "txtTenantID";
            txtTenantID.Size = new Size(800, 55);
            txtTenantID.TabIndex = 5;
            // 
            // lblClientID
            // 
            lblClientID.AutoSize = true;
            lblClientID.ForeColor = Color.FromArgb(220, 220, 220);
            lblClientID.Location = new Point(103, 540);
            lblClientID.Margin = new Padding(2, 0, 2, 0);
            lblClientID.Name = "lblClientID";
            lblClientID.Size = new Size(164, 48);
            lblClientID.TabIndex = 4;
            lblClientID.Text = "Client ID:";
            // 
            // txtClientID
            // 
            txtClientID.BackColor = Color.FromArgb(69, 73, 74);
            txtClientID.BorderStyle = BorderStyle.FixedSingle;
            txtClientID.ForeColor = Color.FromArgb(220, 220, 220);
            txtClientID.Location = new Point(417, 535);
            txtClientID.Margin = new Padding(2);
            txtClientID.Name = "txtClientID";
            txtClientID.Size = new Size(800, 55);
            txtClientID.TabIndex = 0;
            // 
            // lblAccountDetails
            // 
            lblAccountDetails.ForeColor = Color.FromArgb(245, 245, 245);
            lblAccountDetails.Location = new Point(26, 80);
            lblAccountDetails.Name = "lblAccountDetails";
            lblAccountDetails.Size = new Size(1000, 66);
            lblAccountDetails.TabIndex = 4;
            lblAccountDetails.Text = "Account Details";
            // 
            // lblAccountID
            // 
            lblAccountID.AutoSize = true;
            lblAccountID.ForeColor = Color.FromArgb(220, 220, 220);
            lblAccountID.Location = new Point(102, 250);
            lblAccountID.Margin = new Padding(2, 0, 2, 0);
            lblAccountID.Name = "lblAccountID";
            lblAccountID.Size = new Size(203, 48);
            lblAccountID.TabIndex = 10;
            lblAccountID.Text = "Account ID:";
            // 
            // txtAccountID
            // 
            txtAccountID.BackColor = Color.FromArgb(69, 73, 74);
            txtAccountID.BorderStyle = BorderStyle.FixedSingle;
            txtAccountID.ForeColor = Color.FromArgb(220, 220, 220);
            txtAccountID.Location = new Point(417, 246);
            txtAccountID.Margin = new Padding(2);
            txtAccountID.Name = "txtAccountID";
            txtAccountID.Size = new Size(800, 55);
            txtAccountID.TabIndex = 10;
            // 
            // txtAccountName
            // 
            txtAccountName.BackColor = Color.FromArgb(69, 73, 74);
            txtAccountName.BorderStyle = BorderStyle.FixedSingle;
            txtAccountName.ForeColor = Color.FromArgb(220, 220, 220);
            txtAccountName.Location = new Point(417, 170);
            txtAccountName.Margin = new Padding(2);
            txtAccountName.Name = "txtAccountName";
            txtAccountName.ReadOnly = true;
            txtAccountName.Size = new Size(800, 55);
            txtAccountName.TabIndex = 11;
            // 
            // lblAccountName
            // 
            lblAccountName.AutoSize = true;
            lblAccountName.ForeColor = Color.FromArgb(220, 220, 220);
            lblAccountName.Location = new Point(81, 174);
            lblAccountName.Margin = new Padding(2, 0, 2, 0);
            lblAccountName.Name = "lblAccountName";
            lblAccountName.Size = new Size(263, 48);
            lblAccountName.TabIndex = 12;
            lblAccountName.Text = "Account Name:";
            // 
            // AcrylicLabel1
            // 
            AcrylicLabel1.AutoSize = true;
            AcrylicLabel1.ForeColor = Color.FromArgb(220, 220, 220);
            AcrylicLabel1.Location = new Point(417, 313);
            AcrylicLabel1.Margin = new Padding(2, 0, 2, 0);
            AcrylicLabel1.Name = "AcrylicLabel1";
            AcrylicLabel1.Size = new Size(1202, 48);
            AcrylicLabel1.TabIndex = 13;
            AcrylicLabel1.Text = "The Account name cannot be changed on Free Basic. Upgrade to Premium";
            // 
            // AcrylicTitle1
            // 
            AcrylicTitle1.ForeColor = Color.FromArgb(245, 245, 245);
            AcrylicTitle1.Location = new Point(26, 421);
            AcrylicTitle1.Name = "AcrylicTitle1";
            AcrylicTitle1.Size = new Size(1191, 85);
            AcrylicTitle1.TabIndex = 16;
            AcrylicTitle1.Text = "Azure AD Sync";
            // 
            // lblCreateUpdate
            // 
            lblCreateUpdate.AutoSize = true;
            lblCreateUpdate.ForeColor = Color.FromArgb(220, 220, 220);
            lblCreateUpdate.Location = new Point(406, 110);
            lblCreateUpdate.Margin = new Padding(2, 0, 2, 0);
            lblCreateUpdate.Name = "lblCreateUpdate";
            lblCreateUpdate.Size = new Size(1200, 48);
            lblCreateUpdate.TabIndex = 17;
            lblCreateUpdate.Text = "Account Created on {settings.Created}. Last Updated on {settings.Updated}";
            // 
            // lblMessage
            // 
            lblMessage.ForeColor = Color.Crimson;
            lblMessage.ImageAlign = ContentAlignment.BottomLeft;
            lblMessage.Location = new Point(417, 910);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(800, 99);
            lblMessage.TabIndex = 18;
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(20F, 48F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1681, 1231);
            Controls.Add(lblMessage);
            Controls.Add(lblCreateUpdate);
            Controls.Add(AcrylicTitle1);
            Controls.Add(linkLabel1);
            Controls.Add(AcrylicLabel2);
            Controls.Add(btnApplyAdAuth);
            Controls.Add(AcrylicLabel1);
            Controls.Add(btnLogin);
            Controls.Add(txtAccountName);
            Controls.Add(lblTenantID);
            Controls.Add(lblAccountName);
            Controls.Add(lblUsername);
            Controls.Add(txtAccountID);
            Controls.Add(txtTenantID);
            Controls.Add(lblAccountID);
            Controls.Add(lblClientID);
            Controls.Add(lblAccountDetails);
            Controls.Add(txtClientID);
            Controls.Add(lblSettings);
            ForeColor = Color.FromArgb(220, 220, 220);
            Margin = new Padding(5, 6, 5, 6);
            Name = "SettingsForm";
            Text = "Settings";
            Controls.SetChildIndex(lblSettings, 0);
            Controls.SetChildIndex(txtClientID, 0);
            Controls.SetChildIndex(lblAccountDetails, 0);
            Controls.SetChildIndex(lblClientID, 0);
            Controls.SetChildIndex(lblAccountID, 0);
            Controls.SetChildIndex(txtTenantID, 0);
            Controls.SetChildIndex(txtAccountID, 0);
            Controls.SetChildIndex(lblUsername, 0);
            Controls.SetChildIndex(lblAccountName, 0);
            Controls.SetChildIndex(lblTenantID, 0);
            Controls.SetChildIndex(txtAccountName, 0);
            Controls.SetChildIndex(btnLogin, 0);
            Controls.SetChildIndex(AcrylicLabel1, 0);
            Controls.SetChildIndex(btnApplyAdAuth, 0);
            Controls.SetChildIndex(AcrylicLabel2, 0);
            Controls.SetChildIndex(linkLabel1, 0);
            Controls.SetChildIndex(AcrylicTitle1, 0);
            Controls.SetChildIndex(lblCreateUpdate, 0);
            Controls.SetChildIndex(lblMessage, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private AcrylicUI.Controls.AcrylicLabel lblSettings;
        private AcrylicUI.Controls.AcrylicTextBox txtClientID;
        private AcrylicUI.Controls.AcrylicTitle lblUsername;
        private AcrylicUI.Controls.AcrylicButton btnLogin;
        private AcrylicUI.Controls.AcrylicLabel lblTenantID;
        private AcrylicUI.Controls.AcrylicTextBox txtTenantID;
        private AcrylicUI.Controls.AcrylicLabel lblClientID;
        private AcrylicUI.Controls.AcrylicButton btnApplyAdAuth;
        private AcrylicUI.Controls.AcrylicTitle lblAccountDetails;
        private AcrylicUI.Controls.AcrylicLabel lblAccountID;
        private AcrylicUI.Controls.AcrylicTextBox txtAccountID;
        private AcrylicUI.Controls.AcrylicTextBox txtAccountName;
        private AcrylicUI.Controls.AcrylicLabel lblAccountName;
        private AcrylicUI.Controls.AcrylicLabel AcrylicLabel1;
        private AcrylicUI.Controls.AcrylicLabel AcrylicLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private AcrylicUI.Controls.AcrylicTitle AcrylicTitle1;
        private AcrylicUI.Controls.AcrylicLabel lblCreateUpdate;
        private AcrylicUI.Controls.AcrylicLabel lblMessage;
    }
}