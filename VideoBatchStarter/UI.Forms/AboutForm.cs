using AcrylicUI.Forms;
using AcrylicUI.Resources;
using System.Diagnostics;
using System.Reflection;

namespace VideoBatch.UI.Forms
{
    public partial class AboutForm : AcrylicDialog
    {
        private readonly Label lblTitle;
        private readonly Label lblCopyright;
        private readonly LinkLabel lnkWebsite;
        private readonly Label lblVersion;

        public AboutForm()
        {
            // Form settings
            Text = "About VideoBatch";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            DialogButtons = AcrylicDialogButton.Ok;
            IsAcrylic = false;

            // Title Label
            lblTitle = new Label
            {
                Text = "VideoBatch. Edit like a Pro.",
                Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
                ForeColor = Colors.Text,
                AutoSize = true
            };

            // Version Label
            lblVersion = new Label
            {
                Text = $"Version {GetVersion()}",
                ForeColor = Colors.Text,
                AutoSize = true
            };

            // Copyright Label
            lblCopyright = new Label
            {
                Text = "Copyright © 2025 VideoBatch Limited.",
                ForeColor = Colors.Text,
                AutoSize = true
            };

            // Website Link
            lnkWebsite = new LinkLabel
            {
                Text = "www.videobatch.co.uk",
                LinkColor = Colors.ActiveControl,
                ActiveLinkColor = Colors.BlueHighlight,
                VisitedLinkColor = Colors.ActiveControl,
                AutoSize = true
            };
            lnkWebsite.Click += LnkWebsite_Click;

            // Layout
            var padding = 20;
            lblTitle.Location = new Point(padding, padding);
            lblVersion.Location = new Point(padding, lblTitle.Bottom + 10);
            lblCopyright.Location = new Point(padding, lblVersion.Bottom + 20);
            lnkWebsite.Location = new Point(padding, lblCopyright.Bottom + 10);

            // Add controls
            Controls.AddRange(new Control[] { lblTitle, lblVersion, lblCopyright, lnkWebsite });
        }

        private string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return version ?? "1.0.0";
        }

        private void LnkWebsite_Click(object sender, EventArgs e)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://www.videobatch.co.uk",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not open the website. Please visit www.videobatch.co.uk manually.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
} 