﻿using AcrylicUI;
using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Platform.Windows;
using AcrylicUI.Resources;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.InteropServices;
using VideoBatch.UI.Controls;

namespace VideoBatch.UI.Forms
{
    public partial class VideoBatchForm : AcrylicUI.Forms.AcrylicForm
    {


        public VideoBatchForm(
              ILogger<VideoBatchForm> logger,
               ProjectTree projectTree
            )
        {
            _logger = logger;
            _projectTree = projectTree;


            InitializeComponent();
            // Make sure you set AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            // Program.cs : Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            SetupUIDefaults();
            HookEvents();
            RoundCorners(IsWindowsCreatorOrLater());
            DisplayVersion();
            LoadToolWindows();


        }

        private void LoadToolWindows()
        {
            _toolWindows.Add(_projectTree);
            //_toolWindows.Add(this.canvasDock);
            //_toolWindows.Add(this.libraryDock)

            foreach (var toolWindow in _toolWindows)
            {
                DockPanel.AddContent(toolWindow);
            }
        }

        private void DisplayVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var informationVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            _logger.LogInformation ($"InformationalVersion  {informationVersion}");
            // TODO: Configure Logger to log to console by default
            statusLblVersion.Text = $"v:{informationVersion}";

        }


        #region fix FormWindowState changes


        private void SetupUIDefaults()
        {
            // Don't change this: NoBorder with Resize Hack
            var designSize = ClientSize;
            FormBorderStyle = FormBorderStyle.Sizable;
            Size = designSize;
            _restoreSize = designSize; // save for restore
            windowPanel1.ProfileFeature = true;
            windowPanel1.IsAcrylic = false;
            BlurOpacity = 255; // no opacity 
            BackColor = AcrylicUI.Resources.Colors.MontereyDark;

        }

        public bool IsWindowsCreatorOrLater()
        {
            // Create a reference to the OS version of Windows 10 Creators Update.
            Version OsMinVersion = new Version(10, 0, 15063, 0);

            // Compare the current version to the minimum required version.
            var compatible = Environment.OSVersion.Version.CompareTo(OsMinVersion);
            Console.WriteLine($"{Environment.OSVersion.VersionString}: Compat: {compatible}");

            return compatible == 1;
        }
        private void HookEvents()
        {
            Load += new EventHandler(MainWindow_Load!);
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var dpiScale = IconFactory.GetDpiScale(Handle);
            windowPanel1.Icon = new IconFactory(IconFactory.GetDpiScale(Handle)).BitmapFromSvg(Icons.Cube_16x_svg);
            windowPanel1.SectionHeader = "VideoBatch";
        }


        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            _restoreSize = ClientSize;
            WindowState = (WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
            AdjustForm();
        }

        #endregion

        #region Min/Max/Restore for catching resize events to adjust form

        private void BtnMin_Click(object sender, EventArgs e)
        {
            _restoreSize = ClientSize;
            WindowState = FormWindowState.Minimized;
            AdjustForm();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustForm();
        }

        private void AdjustForm()
        {
            switch (WindowState)
            {
                case FormWindowState.Maximized: //Maximized form (After)
                    Padding = new Padding(8, 8, 8, 8);
                    break;
                case FormWindowState.Normal: //Restored form (After)
                    if (Padding.Top != borderSize)
                        Padding = new Padding(borderSize);
                    break;
            }
        }



        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Clicks == 2) // DoubleClick
            {
                BtnMaximize_Click(this, e);
            }
            else
            {
                WinProcExtentsions.TitleBarHit(Handle);
            }
        }


        #endregion

        #region Windows AcrylicTheme Hack

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Win32Hacks.DarkThemeTitleBar(Handle);
        }

        #endregion

        #region Window, No Border Hacks

        protected override void WndProc(ref Message message)
        {
            // Resize Window
            if (message.Msg == WinUserH.WM_NCHITTEST)
            {
                base.WndProc(ref message);

                if (WindowState == FormWindowState.Normal)
                {
                    if ((int)message.Result == WinUserH.HT_CLIENT)
                    {
                        var cursor = PointToClient(Cursor.Position);
                        WindowPanel.CheckResizeClientAreaHit(ClientSize, ref message, cursor);
                    }
                }
                return;
            }

            // Remove border and keep snap window
            if (message.Msg == WinUserH.WM_NCCALCSIZE && message.WParam.ToInt32() == 1)
            {
                return;
            }

            // Keep form size when it is minimized and restored.            
            if (message.Msg == WinUserH.WM_SYSCOMMAND)
            {
                int wParam = (message.WParam.ToInt32() & 0xFFF0);
                if (wParam == WinUserH.SC_MINIMIZE)
                {
                    //save client size
                    _restoreSize = ClientSize;
                }
                if (wParam == WinUserH.SC_RESTORE)
                {
                    // restore client size
                    Size = _restoreSize;
                }
            }

            base.WndProc(ref message);
        }



        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (!_flatBorder)
                return;

            var g = e.Graphics;

            using (var p = new Pen(Colors.DarkBorder))
            {
                var modRect = new Rectangle(ClientRectangle.Location, new Size(ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                g.DrawRectangle(p, modRect);
            }
        }

        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// Add DropShadow to top level windows
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        #endregion

        #region Round Corners


        private void RoundCorners(bool _isWindows11)
        {
            if (_isWindows11)
            {
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                DwmSetWindowAttribute(Handle, attribute, ref preference, sizeof(uint));
            }
            windowPanel1.RoundCorners = _isWindows11;
        }

        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern long DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);

        #endregion

        #region Fields for Borderless Windows
        private int borderSize = 0;
        private bool _flatBorder = true;
        private Size _restoreSize;
        #endregion

        #region Fields for ToolWindows
        //private readonly MediaDock mediaDock;
        //private readonly CanvasDock canvasDock;
        //private readonly LibraryDock libraryDock;
        private readonly List<DockContent> _toolWindows = new List<DockContent>();
        private readonly ILogger<VideoBatchForm> _logger;
        private readonly ProjectTree _projectTree;
        #endregion

    }
}
