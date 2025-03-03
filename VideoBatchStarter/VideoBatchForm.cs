using AcrylicUI;
using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Platform.Windows;
using AcrylicUI.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.InteropServices;
using VideoBatch.Services;
using VideoBatch.UI.Controls;
using VideoBatch.UI.Forms.Docking;

namespace VideoBatch.UI.Forms
{
    public partial class VideoBatchForm : AcrylicUI.Forms.AcrylicForm
    {
        private readonly ILogger<VideoBatchForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProjectTree _projectTree;
        private readonly IDocumentationService _documentationService;

        // New docking panels
        private readonly MediaInspectorDock _mediaInspector;
        private readonly BatchProcessingDock _batchProcessing;
        private readonly OutputDock _output;

        private readonly DockPanel _dockPanel;
        private readonly Dictionary<string, DockContent> _toolWindows;
        private readonly Dictionary<string, ToolStripMenuItem> _toolWindowMenuItems;

        public VideoBatchForm(
              ILogger<VideoBatchForm> logger,
              IServiceProvider serviceProvider,
              ProjectTree projectTree,
              IDocumentationService documentationService
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _projectTree = projectTree;
            _documentationService = documentationService;

            // Initialize docking panels
            _mediaInspector = new MediaInspectorDock();
            _batchProcessing = new BatchProcessingDock();
            _output = new OutputDock();

            _dockPanel = new DockPanel();
            _toolWindows = new Dictionary<string, DockContent>();
            _toolWindowMenuItems = new Dictionary<string, ToolStripMenuItem>();

            InitializeComponent();
            // Make sure you set AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            // Program.cs : Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            SetupMenuItems();
            SetupUIDefaults();
            HookEvents();
            RoundCorners(IsWindowsCreatorOrLater());
            DisplayVersion();
            LoadToolWindows();
        }

        private void SetupMenuItems()
        {
            // Define AcrylicUI colors
            var menuBackColor = Color.FromArgb(31, 31, 31);
            var menuForeColor = Color.FromArgb(220, 220, 220);

            // Setup File menu items
            var newProjectItem = new ToolStripMenuItem("&New Project...", null, new EventHandler(NewProject_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.N
            };
            
            var openProjectItem = new ToolStripMenuItem("&Open Project...", null, new EventHandler(OpenProject_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.O
            };
            
            var saveProjectItem = new ToolStripMenuItem("&Save Project", null, new EventHandler(SaveProject_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.S
            };
            
            var saveProjectAsItem = new ToolStripMenuItem("Save Project &As...", null, new EventHandler(SaveProjectAs_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.S
            };

            var recentProjectsItem = new ToolStripMenuItem("Recent Pro&jects")
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor
            };
            
            var exitMenuItem = new ToolStripMenuItem("E&xit", null, new EventHandler(ExitMenuItem_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Alt | Keys.X
            };

            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                newProjectItem,
                openProjectItem,
                saveProjectItem,
                saveProjectAsItem,
                new ToolStripSeparator() { BackColor = menuBackColor },
                recentProjectsItem,
                new ToolStripSeparator() { BackColor = menuBackColor },
                exitMenuItem
            });

            // Setup Edit menu
            var editMenu = new ToolStripMenuItem("&Edit") { BackColor = menuBackColor, ForeColor = menuForeColor };
            var undoItem = new ToolStripMenuItem("&Undo", null, new EventHandler(Undo_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.Z
            };
            
            var redoItem = new ToolStripMenuItem("&Redo", null, new EventHandler(Redo_Click))
            {
                BackColor = menuBackColor,
                ForeColor = menuForeColor,
                ShortcutKeys = Keys.Control | Keys.Y
            };

            editMenu.DropDownItems.AddRange(new ToolStripItem[] {
                undoItem,
                redoItem,
                new ToolStripSeparator() { BackColor = menuBackColor },
                new ToolStripMenuItem("Cu&t", null, new EventHandler(Cut_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor, 
                    ShortcutKeys = Keys.Control | Keys.X 
                },
                new ToolStripMenuItem("&Copy", null, new EventHandler(Copy_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor, 
                    ShortcutKeys = Keys.Control | Keys.C 
                },
                new ToolStripMenuItem("&Paste", null, new EventHandler(Paste_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor, 
                    ShortcutKeys = Keys.Control | Keys.V 
                },
                new ToolStripMenuItem("&Delete", null, new EventHandler(Delete_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor, 
                    ShortcutKeys = Keys.Delete 
                }
            });

            // Setup View menu
            var viewMenu = new ToolStripMenuItem("&View") { BackColor = menuBackColor, ForeColor = menuForeColor };
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Project &Explorer", null, new EventHandler(ToggleProjectExplorer_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor,
                    Checked = true, 
                    CheckOnClick = true 
                },
                new ToolStripMenuItem("&Media Inspector", null, new EventHandler(ToggleMediaInspector_Click))
                {
                    BackColor = menuBackColor,
                    ForeColor = menuForeColor,
                    Checked = true,
                    CheckOnClick = true
                },
                new ToolStripMenuItem("&Batch Processing", null, new EventHandler(ToggleBatchProcessing_Click))
                {
                    BackColor = menuBackColor,
                    ForeColor = menuForeColor,
                    Checked = true,
                    CheckOnClick = true
                },
                new ToolStripMenuItem("&Output Window", null, new EventHandler(ToggleOutput_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor,
                    Checked = true, 
                    CheckOnClick = true 
                },
                new ToolStripSeparator() { BackColor = menuBackColor },
                // Full screen functionality temporarily disabled
                //new ToolStripMenuItem("&Full Screen", null, new EventHandler(ToggleFullScreen_Click)) 
                //{ 
                //    BackColor = menuBackColor, 
                //    ForeColor = menuForeColor,
                //    ShortcutKeys = Keys.F11 
                //}
            });

            // Setup Help menu
            var helpMenu = new ToolStripMenuItem("&Help") { BackColor = menuBackColor, ForeColor = menuForeColor };
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("&Documentation", null, new EventHandler(ShowDocumentation_Click)) 
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor,
                    ShortcutKeys = Keys.F1 
                },
                new ToolStripMenuItem("Check for &Updates", null, new EventHandler(CheckUpdates_Click))
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor
                },
                new ToolStripSeparator() { BackColor = menuBackColor },
                new ToolStripMenuItem("&About VideoBatch", null, new EventHandler(ShowAbout_Click))
                { 
                    BackColor = menuBackColor, 
                    ForeColor = menuForeColor
                }
            });

            // Setup Options menu
            var optionsMenu = new ToolStripMenuItem("&Options") { BackColor = menuBackColor, ForeColor = menuForeColor };
            optionsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("&Settings...", null, new EventHandler(ShowSettings_Click))
                {
                    BackColor = menuBackColor,
                    ForeColor = menuForeColor
                }
            });

            // Add all menus to the menu strip
            menuStrip.Items.AddRange(new ToolStripItem[] {
                fileToolStripMenuItem,
                editMenu,
                viewMenu,
                optionsMenu,
                helpMenu
            });
        }

        #region Menu Event Handlers
        private void NewProject_Click(object sender, EventArgs e) => _logger.LogInformation("New Project clicked");
        private void OpenProject_Click(object sender, EventArgs e) => _logger.LogInformation("Open Project clicked");
        private void SaveProject_Click(object sender, EventArgs e) => _logger.LogInformation("Save Project clicked");
        private void SaveProjectAs_Click(object sender, EventArgs e) => _logger.LogInformation("Save Project As clicked");
        private void Undo_Click(object sender, EventArgs e) => _logger.LogInformation("Undo clicked");
        private void Redo_Click(object sender, EventArgs e) => _logger.LogInformation("Redo clicked");
        private void Cut_Click(object sender, EventArgs e) => _logger.LogInformation("Cut clicked");
        private void Copy_Click(object sender, EventArgs e) => _logger.LogInformation("Copy clicked");
        private void Paste_Click(object sender, EventArgs e) => _logger.LogInformation("Paste clicked");
        private void Delete_Click(object sender, EventArgs e) => _logger.LogInformation("Delete clicked");
        private void ToggleProjectExplorer_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Project Explorer clicked");
            ToggleToolWindow(_projectTree);
        }
        private void ToggleMediaInspector_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Media Inspector clicked");
            ToggleToolWindow(_mediaInspector);
        }
        private void ToggleBatchProcessing_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Batch Processing clicked");
            ToggleToolWindow(_batchProcessing);
        }
        private void ToggleOutput_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Output clicked");
            ToggleToolWindow(_output);
        }
        // Full screen functionality temporarily disabled
        //private void ToggleFullScreen_Click(object sender, EventArgs e) => _logger.LogInformation("Toggle Full Screen clicked");
        private async void ShowDocumentation_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Show Documentation clicked");
            try
            {
                await _documentationService.ShowDocumentationAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing documentation");
                MessageBox.Show(
                    "Could not open documentation. Please make sure the README.md file exists.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void CheckUpdates_Click(object sender, EventArgs e) => _logger.LogInformation("Check Updates clicked");
        private void ShowAbout_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Show About clicked");
            AboutForm.Show(this);
        }

        private void ShowSettings_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Opening Settings dialog");
            var settingsForm = new SettingsForm(_serviceProvider.GetRequiredService<ILogger<SettingsForm>>());
            settingsForm.ShowDialog(this);
        }

        private void ToggleToolWindow(DockContent toolWindow)
        {
            if (toolWindow.DockPanel == null)
            {
                DockPanel.AddContent(toolWindow);
                UpdateMenuItemState(toolWindow, true);
            }
            else
            {
                DockPanel.RemoveContent(toolWindow);
                UpdateMenuItemState(toolWindow, false);
            }
        }

        private void UpdateMenuItemState(DockContent toolWindow, bool isVisible)
        {
            var menuItem = GetMenuItemForToolWindow(toolWindow);
            if (menuItem != null)
            {
                menuItem.Checked = isVisible;
            }
        }

        private ToolStripMenuItem? GetMenuItemForToolWindow(DockContent toolWindow)
        {
            if (toolWindow == _projectTree)
                return GetViewMenuItem("Project Explorer");
            else if (toolWindow == _mediaInspector)
                return GetViewMenuItem("Media Inspector");
            else if (toolWindow == _batchProcessing)
                return GetViewMenuItem("Batch Processing");
            else if (toolWindow == _output)
                return GetViewMenuItem("Output Window");
            return null;
        }

        private ToolStripMenuItem? GetViewMenuItem(string text)
        {
            var viewMenu = menuStrip.Items.OfType<ToolStripMenuItem>().FirstOrDefault(x => x.Text == "&View");
            return viewMenu?.DropDownItems.OfType<ToolStripMenuItem>().FirstOrDefault(x => x.Text == $"Project &Explorer" && text == "Project Explorer" ||
                                                                                          x.Text == "&Media Inspector" && text == "Media Inspector" ||
                                                                                          x.Text == "&Batch Processing" && text == "Batch Processing" ||
                                                                                          x.Text == "&Output Window" && text == "Output Window");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Hook up to DockPanel events
            DockPanel.ContentRemoved += DockPanel_ContentRemoved;
        }

        private void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            // Update menu item state when content is removed
            UpdateMenuItemState(e.Content, false);
        }
        #endregion

        private void LoadToolWindows()
        {
            // First add side panels (left and right)
            _projectTree.DefaultDockArea = DockArea.Left;
            _projectTree.DockArea = DockArea.Left;
            _projectTree.Width = 250;
            DockPanel.AddContent(_projectTree);

            _batchProcessing.DefaultDockArea = DockArea.Right;
            _batchProcessing.DockArea = DockArea.Right;
            _batchProcessing.Width = 300;
            DockPanel.AddContent(_batchProcessing);

            // Then add bottom panels - they will appear as tabs since they share the same dock area
            _mediaInspector.DefaultDockArea = DockArea.Bottom;
            _mediaInspector.DockArea = DockArea.Bottom;
            _mediaInspector.Height = 200;
            DockPanel.AddContent(_mediaInspector);

            _output.DefaultDockArea = DockArea.Bottom;
            _output.DockArea = DockArea.Bottom;
            _output.Height = 200;
            DockPanel.AddContent(_output);

            // Add to tool windows list for management
            _toolWindows.Add("ProjectTree", _projectTree);
            _toolWindows.Add("BatchProcessing", _batchProcessing);
            _toolWindows.Add("MediaInspector", _mediaInspector);
            _toolWindows.Add("Output", _output);
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
        #endregion

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
