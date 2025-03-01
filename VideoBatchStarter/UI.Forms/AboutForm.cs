using AcrylicUI.Forms;
using AcrylicUI.Resources;
using System.Diagnostics;
using System.Reflection;

namespace VideoBatch.UI.Forms
{
    public partial class AboutForm : AcrylicForm
    {
        private readonly Label lblTitle;
        private readonly Label lblCopyright;
        private readonly LinkLabel lnkWebsite;
        private readonly Label lblVersion;
        private readonly Button btnOk;

        public AboutForm()
        {
            // Form settings
            Text = "About VideoBatch";
            Size = new Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Colors.GreyBackground;
            ForeColor = Colors.Text;

            // Title Label
            lblTitle = new Label
            {
                Text = "VideoBatch. Edit like a Pro.",
                Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                ForeColor = Colors.Text,
                AutoSize = false,
                BackColor = Colors.GreyBackground,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = ClientSize.Width
            };

            // Version Label
            lblVersion = new Label
            {
                Text = $"Version {GetVersion()}",
                Font = new Font(Font.FontFamily, 8, FontStyle.Regular),
                ForeColor = Colors.Text,
                AutoSize = false,
                BackColor = Colors.GreyBackground,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = ClientSize.Width
            };

            // Copyright Label
            lblCopyright = new Label
            {
                Text = "Copyright © 2025 VideoBatch Limited.",
                Font = new Font(Font.FontFamily, 8, FontStyle.Regular),
                ForeColor = Colors.Text,
                AutoSize = false,
                BackColor = Colors.GreyBackground,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = ClientSize.Width
            };

            // Website Link
            lnkWebsite = new LinkLabel
            {
                Text = "www.videobatch.co.uk",
                Font = new Font(Font.FontFamily, 8, FontStyle.Regular),
                LinkColor = Colors.ActiveControl,
                ActiveLinkColor = Colors.BlueHighlight,
                VisitedLinkColor = Colors.ActiveControl,
                AutoSize = false,
                BackColor = Colors.GreyBackground,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = ClientSize.Width
            };
            lnkWebsite.Click += LnkWebsite_Click;

            // OK Button
            btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new Size(150, 46),
                Font = new Font(Font.FontFamily, 8, FontStyle.Regular),
                BackColor = Colors.GreyBackground,
                ForeColor = Colors.Text
            };

            // Layout with increased padding
            var padding = 40;
            var verticalSpacing = 20;

            lblTitle.Location = new Point(0, padding);
            lblVersion.Location = new Point(0, lblTitle.Bottom + verticalSpacing);
            lblCopyright.Location = new Point(0, lblVersion.Bottom + verticalSpacing * 2);
            lnkWebsite.Location = new Point(0, lblCopyright.Bottom + verticalSpacing);
            btnOk.Location = new Point((ClientSize.Width - btnOk.Width) / 2, ClientSize.Height - btnOk.Height - padding);

            // Set control heights
            lblTitle.Height = 30;
            lblVersion.Height = 20;
            lblCopyright.Height = 20;
            lnkWebsite.Height = 20;

            // Add controls
            Controls.AddRange(new Control[] { lblTitle, lblVersion, lblCopyright, lnkWebsite, btnOk });

            // Set accept button
            AcceptButton = btnOk;
        }

        private string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
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