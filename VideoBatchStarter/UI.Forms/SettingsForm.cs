using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using AcrylicUI.Forms;
using Microsoft.Extensions.Logging;

namespace VideoBatch.UI.Forms
{
    public partial class SettingsForm : AcrylicDialog
    {
        private readonly ILogger<SettingsForm> _logger;
        private readonly Size btnSize = new(75, 25);
        private bool _isDirty = false;

        private bool _loggedIn = false;
        private bool _isAzureADAppRegistered = false; 

        public SettingsForm(ILogger<SettingsForm> logger)
        {
            _logger = logger;

            InitializeComponent();
            this.DialogButtons = AcrylicDialogButton.OkCancel;
            this.btnOk.Text = "Apply";
            this.btnLogin.Visible = false;
            this.IsAcrylic = false;
            
            // Set dark theme colors
            this.BackColor = AcrylicUI.Resources.Colors.GreyBackground;
            this.ForeColor = Color.FromArgb(220, 220, 220);
            
            // Ensure all textboxes have consistent dark theme
            foreach (Control control in this.Controls)
            {
                if (control is AcrylicUI.Controls.AcrylicTextBox textBox)
                {
                    textBox.BackColor = Color.FromArgb(69, 73, 74);
                    textBox.ForeColor = Color.FromArgb(220, 220, 220);
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            
            // Commented out Azure AD specific code for now
            //this.txtClientID.Text = "5a9a..";
            //this.txtTenantID.Text = "7d75...";

            HookEvents();
        }

        private void HookEvents()
        {
            this.Load += FrmSettings_Load;
            this.btnOk.Click += BtnOk_Click;
            this.btnCancel.Click += BtnCancel_Click;
            this.btnLogin.Click += BtnLoginLogout_ClickAsync;
            this.linkLabel1.LinkClicked += LnkUpgrade1_LinkClicked;
            this.btnApplyAdAuth.Click += BtnApplyAdAuth_Click;
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            _logger.LogInformation("Settings form loaded");
            
            // Commented out dashboard settings for now
            //this.txtAccountID.Text = _dashboardSettings?.AccountID.ToString();
            //this.txtAccountName.Text = _dashboardSettings.Name;
            //var strCreateDate = $"Account Created on {_dashboardSettings.DateCreated}. Last Updated on {_dashboardSettings.DateUpdated}";
            //this.lblCreateUpdate.Text = strCreateDate;

            if (_isAzureADAppRegistered == true)
            {
                SetupAzureAD_UI();
            }
            else
            {
                ClearAzureAD_UI();
            }
        }

        private void BtnLoginLogout_ClickAsync(object sender, EventArgs e)
        {
            if (!_loggedIn) // then Login
            {
                //Windows Auth
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new(current);
                lblUsername.Text = windowsPrincipal.Identity.Name;

                // Commented out Azure AD login for now
                //var authResult = await AuthAzureAD.Login();
                //lblUsername.Text = authResult.Account.Username;

                btnLogin.Text = "Log Out";
                _loggedIn = true;
            }
            else // then Logout
            {
                // Commented out Azure AD logout for now
                //await AuthAzureAD.Logout();
                btnLogin.Text = "Log In";
                _loggedIn = false;
            }
        }

        private void SetupAzureAD_UI()
        {
            _isAzureADAppRegistered = true;
            btnApplyAdAuth.Text = "Registered";
            btnApplyAdAuth.Enabled = false;
            this.btnLogin.Visible = true;
            this.btnLogin.Size = btnSize;
            this.txtClientID.ReadOnly = true;
            this.txtTenantID.ReadOnly = true;
        }

        private void ClearAzureAD_UI()
        {
            _isAzureADAppRegistered = false;
            btnApplyAdAuth.Text = "Register Azure AD";
            btnApplyAdAuth.Enabled = true;
            this.btnLogin.Visible = false;
            this.txtClientID.ReadOnly = false;
            this.txtTenantID.ReadOnly = false;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (_isDirty)
            {
                _logger.LogInformation("Saving settings");
                _logger.LogInformation("Settings Saved");
            }
            this.Close();
            _logger.LogInformation("Closing");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Closing without saving settings");
            this.Close();
        }

        private void LnkUpgrade1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            string target = "https://www.videobatch.co.uk";
            try
            {
                OpenUrl(target);
                _logger.LogInformation("OpenUrl Clicked for {target}", target);
            }
            catch
            {
                _logger.LogError("Failed to open browser");
            }
        }

        private static void OpenUrl(string url)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception)
            {
                // If the above fails, try platform-specific approaches
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void BtnApplyAdAuth_Click(object sender, EventArgs e)
        {
            try
            {
                // Commented out Azure AD initialization for now
                //if (AuthAzureAD.Initialize(this.txtClientID.Text, this.txtTenantID.Text))
                //{
                //    SetupAzureAD_UI();
                //}
                _logger.LogInformation("Azure AD authentication not implemented yet");
                lblMessage.Text = "Azure AD authentication not implemented yet";
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                ClearAzureAD_UI();
            }
        }
    }
}
