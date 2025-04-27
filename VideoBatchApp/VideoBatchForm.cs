using AcrylicUI;
using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Platform.Windows;
using AcrylicUI.Resources;
using AcrylicUI.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using VideoBatch.Services;
using VideoBatchApp.Services;
using VideoBatch.UI.Controls;
using VideoBatch.UI.Forms.Docking;
using VideoBatch.Model;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using VideoBatchApp; // Added for IWorkAreaFactory namespace

namespace VideoBatch.UI.Forms
{
    public partial class VideoBatchForm : AcrylicUI.Forms.AcrylicForm
    {
        private readonly ILogger<VideoBatchForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProjectTree _projectTree;
        private readonly IDocumentationService _documentationService;
        private readonly IWorkAreaFactory _workAreaFactory; // Added WorkAreaFactory
        private readonly IRecentFilesService _recentFilesService; // Inject Recent Files Service

        // New docking panels
        private readonly AssetsDock _assets;
        private readonly BatchProcessingDock _batchProcessing;
        private readonly OutputDock _output;

        private readonly Dictionary<string, DockContent> _toolWindows;
        private readonly Dictionary<string, ToolStripMenuItem> _toolWindowMenuItems;

        public VideoBatchForm(
              ILogger<VideoBatchForm> logger,
              IServiceProvider serviceProvider,
              ProjectTree projectTree,
              IDocumentationService documentationService,
              IWorkAreaFactory workAreaFactory, // Inject WorkAreaFactory
              IRecentFilesService recentFilesService // Inject Recent Files Service
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _projectTree = projectTree;
            _documentationService = documentationService;
            _workAreaFactory = workAreaFactory; // Store WorkAreaFactory
            _recentFilesService = recentFilesService; // Store Recent Files Service

            // Initialize docking panels
            _assets = new AssetsDock();
            _batchProcessing = new BatchProcessingDock();
            _output = new OutputDock();

            _toolWindows = new Dictionary<string, DockContent>();
            _toolWindowMenuItems = new Dictionary<string, ToolStripMenuItem>();

            InitializeComponent();

            // Add message filters for docking resize and drag
            Application.AddMessageFilter(DockPanel.DockContentDragFilter);
            Application.AddMessageFilter(DockPanel.DockResizeFilter);

            // Set initial sizes for dock regions (optional, but good practice)
            // DockPanel.Regions[DockArea.Left].Size = new System.Drawing.Size(300, 0); // Removed - Width set on control directly
            DockPanel.Regions[DockArea.Bottom].Size = new System.Drawing.Size(0, 350); // Changed height from 150

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
                new ToolStripMenuItem("&Assets", null, new EventHandler(ToggleAssets_Click))
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

            // Hook event for dynamic menu population
            fileToolStripMenuItem.DropDownOpening += FileToolStripMenuItem_DropDownOpening;
        }

        #region Menu Event Handlers
        private void NewProject_Click(object? sender, EventArgs e)
        {
            using (var newProjectForm = new NewProjectForm())
            {
                if (newProjectForm.ShowDialog() == DialogResult.OK)
                {
                    var projectName = newProjectForm.ProjectName;
                    var projectLocation = newProjectForm.ProjectLocation;
                    // Ensure .json extension (already handled in NewProjectForm.OnFormClosing, but good practice to double-check)
                    if (!projectName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        projectName += ".json";
                    }
                    var projectPath = Path.Combine(projectLocation, projectName);

                    // --- Overwrite Check --- 
                    if (File.Exists(projectPath))
                    {
                        var result = AcrylicMessageBox.ShowWarning(
                            $"The file '{projectName}' already exists in the selected location.\nDo you want to replace it?",
                            "Confirm Overwrite",
                            AcrylicDialogButton.YesNo);

                        if (result != DialogResult.Yes)
                        {
                            _logger.LogInformation("User chose not to overwrite existing project file.");
                            return; // User canceled overwrite
                        }
                        _logger.LogWarning($"Overwriting existing project file: {projectPath}");
                    }
                    // --- End Overwrite Check ---

                    try
                    {
                        // Create a new project file
                        var project = new Project
                        {
                            Name = projectName,
                            DateCreated = SystemClock.Instance.GetCurrentInstant(),
                            DateUpdated = SystemClock.Instance.GetCurrentInstant()
                        };

                        // Save the project with pretty-printed JSON and NodaTime support
                        var options = new System.Text.Json.JsonSerializerOptions { 
                            WriteIndented = true
                        }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                        
                        var json = System.Text.Json.JsonSerializer.Serialize(project, options);
                        File.WriteAllText(projectPath, json);

                        // Update the project tree
                        // _projectTree.BuildTreeView(); // Commented out: BuildTreeView might be obsolete/incorrect
                        _logger.LogInformation("Created new project: {projectPath}. ProjectTree needs manual refresh/reload.", projectPath);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating new project");
                        MessageBox.Show($"Error creating project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to load project data from the specified file path.
        /// </summary>
        /// <param name="filePath">The full path to the project JSON file.</param>
        private async Task LoadProjectAsync(string filePath)
        {
            _logger.LogInformation("Attempting to load project from: {FilePath}", filePath);
            try
            {
                // Resolve the data service from the service provider
                var dataService = _serviceProvider.GetRequiredService<IDataService>();

                // Load the data - this caches it in the service
                Account loadedAccount = await dataService.LoadDataAsync(filePath);
                 _logger.LogInformation("Successfully loaded data for account: {AccountName}", loadedAccount.Name);

                // Add to recent files list *after* successful load
                _recentFilesService.AddRecentFile(filePath);
                // PopulateRecentFilesMenu(); // Refresh menu (will add this call later)

                // Trigger ProjectTree population
                 _logger.LogInformation("Populating Project Tree.");
                _projectTree.LoadAndPopulateTree();

                // Update status bar or window title
                this.Text = $"VideoBatch - {System.IO.Path.GetFileNameWithoutExtension(filePath)}";
                // statusBarLabel.Text = $"Project '{loadedAccount.Name}' loaded.";
            }
            catch (FileNotFoundException fnfEx)
            {
                _logger.LogError(fnfEx, "Selected project file not found: {FilePath}", filePath);
                MessageBox.Show(this, $"Error loading project:\nThe specified file was not found.\n\n{filePath}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to parse project file: {FilePath}", filePath);
                MessageBox.Show(this, $"Error loading project:\nThe file contains invalid JSON data or doesn't match the expected format.\n\n{jsonEx.Message}", "Invalid Project File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while loading project file: {FilePath}", filePath);
                MessageBox.Show(this, $"An unexpected error occurred while loading the project.\n\n{ex.Message}", "Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OpenProject_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Open Project clicked");

            using var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Project Files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Open Project File",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true // Remember the last directory
            };

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var filePath = openFileDialog.FileName;
                _logger.LogInformation("User selected project file: {FilePath}", filePath);
                await LoadProjectAsync(filePath); // Call the refactored method
            }
            else
            {
                _logger.LogInformation("User cancelled Open Project dialog.");
            }
        }
        private void SaveProject_Click(object? sender, EventArgs e) => _logger.LogInformation("Save Project clicked");
        private void SaveProjectAs_Click(object? sender, EventArgs e) => _logger.LogInformation("Save Project As clicked");
        private void Undo_Click(object? sender, EventArgs e) => _logger.LogInformation("Undo clicked");
        private void Redo_Click(object? sender, EventArgs e) => _logger.LogInformation("Redo clicked");
        private void Cut_Click(object? sender, EventArgs e) => _logger.LogInformation("Cut clicked");
        private void Copy_Click(object? sender, EventArgs e) => _logger.LogInformation("Copy clicked");
        private void Paste_Click(object? sender, EventArgs e) => _logger.LogInformation("Paste clicked");
        private void Delete_Click(object? sender, EventArgs e) => _logger.LogInformation("Delete clicked");
        private void ToggleProjectExplorer_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Project Explorer clicked");
            ToggleToolWindow(_projectTree);
        }
        private void ToggleAssets_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Assets clicked");
            ToggleToolWindow(_assets);
        }
        private void ToggleBatchProcessing_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Batch Processing clicked");
            ToggleToolWindow(_batchProcessing);
        }
        private void ToggleOutput_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Toggle Output clicked");
            ToggleToolWindow(_output);
        }
        // Full screen functionality temporarily disabled
        //private void ToggleFullScreen_Click(object? sender, EventArgs e) => _logger.LogInformation("Toggle Full Screen clicked");
        private async void ShowDocumentation_Click(object? sender, EventArgs e)
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
        private void CheckUpdates_Click(object? sender, EventArgs e) => _logger.LogInformation("Check Updates clicked");
        private void ShowAbout_Click(object? sender, EventArgs e)
        {
            _logger.LogInformation("Show About clicked");
            AboutForm.Show(this);
        }

        private void ShowSettings_Click(object? sender, EventArgs e)
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
            else if (toolWindow == _assets)
                return GetViewMenuItem("Assets");
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
                                                                                          x.Text == "&Assets" && text == "Assets" ||
                                                                                          x.Text == "&Batch Processing" && text == "Batch Processing" ||
                                                                                          x.Text == "&Output Window" && text == "Output Window");
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Hook up to DockPanel events
            DockPanel.ContentRemoved += DockPanel_ContentRemoved;
            // Populate recent files on initial load
            PopulateRecentFilesMenu();

            // Auto-load the most recent project if available
            var recentFiles = _recentFilesService.GetRecentFiles();
            if (recentFiles.Any()) // Check if the list is not empty
            {
                var mostRecentProject = recentFiles.First(); // Get the first (most recent) path
                _logger.LogInformation("Found recent project(s). Auto-loading most recent: {FilePath}", mostRecentProject);
                await LoadProjectAsync(mostRecentProject); // Load it
            }
            else
            {
                 _logger.LogInformation("No recent projects found to auto-load.");
            }
        }

        private void DockPanel_ContentRemoved(object? sender, DockContentEventArgs e)
        {
            // Update menu item state when content is removed
            UpdateMenuItemState(e.Content, false);
        }

        // Handles the File menu opening to refresh the recent projects list
        private void FileToolStripMenuItem_DropDownOpening(object? sender, EventArgs e)
        {
            PopulateRecentFilesMenu();
        }

        // Populates the "Recent Projects" submenu
        private void PopulateRecentFilesMenu()
        {
            var recentProjectsItem = fileToolStripMenuItem.DropDownItems
                                          .OfType<ToolStripMenuItem>()
                                          .FirstOrDefault(item => item.Text == "Recent Pro&jects");

            if (recentProjectsItem == null)
            {
                _logger.LogError("Could not find 'Recent Projects' menu item to populate.");
                return;
            }

            // Clear existing recent file entries (excluding separators or placeholders)
            var itemsToRemove = recentProjectsItem.DropDownItems.OfType<ToolStripMenuItem>().ToList();
            foreach (var item in itemsToRemove)
            {
                recentProjectsItem.DropDownItems.Remove(item);
                item.Dispose(); // Dispose the old item
            }

            var recentFiles = _recentFilesService.GetRecentFiles();

            if (!recentFiles.Any())
            {
                recentProjectsItem.DropDownItems.Add(new ToolStripMenuItem("(No recent projects)") { Enabled = false });
            }
            else
            {
                for (int i = 0; i < recentFiles.Count; i++)
                {
                    var path = recentFiles[i];
                    var menuItemText = $"&{i + 1} {Path.GetFileName(path)}";
                    var menuItem = new ToolStripMenuItem(menuItemText);
                    menuItem.Tag = path; // Store the full path in the Tag
                    menuItem.ToolTipText = path; // Show full path on hover
                    menuItem.Click += RecentFile_Click; // Add click handler
                    recentProjectsItem.DropDownItems.Add(menuItem);
                }
            }
        }

        // Handles clicking on a recent project menu item
        private async void RecentFile_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is string filePath)
            {
                await LoadProjectAsync(filePath);
            }
        }
        #endregion

        private void LoadToolWindows()
        {
            // First add side panels (left and right)
            _projectTree.DefaultDockArea = DockArea.Left;
            _projectTree.DockArea = DockArea.Left;
            _projectTree.Width = 300;
            this.DockPanel.AddContent(_projectTree);

            _batchProcessing.DefaultDockArea = DockArea.Right;
            _batchProcessing.DockArea = DockArea.Right;
            _batchProcessing.Width = 300;
            this.DockPanel.AddContent(_batchProcessing);

            // Then add bottom panels - they will appear as tabs since they share the same dock area
            _assets.DefaultDockArea = DockArea.Bottom;
            _assets.DockArea = DockArea.Bottom;
            _assets.Height = 350;
            this.DockPanel.AddContent(_assets);

            _output.DefaultDockArea = DockArea.Bottom;
            _output.DockArea = DockArea.Bottom;
            _output.Height = 350;
            this.DockPanel.AddContent(_output);

            // Add to tool windows list for management
            _toolWindows.Add("ProjectTree", _projectTree);
            _toolWindows.Add("BatchProcessing", _batchProcessing);
            _toolWindows.Add("Assets", _assets);
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
            Load += MainWindow_Load;
            DockPanel.ContentRemoved += DockPanel_ContentRemoved; // Example if needed
            
            // Wire up the ProjectTree DoubleClick event
            _projectTree.DoubleClick += ProjectTree_DoubleClick; 
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var dpiScale = IconFactory.GetDpiScale(Handle);
            windowPanel1.Icon = new IconFactory(IconFactory.GetDpiScale(Handle)).BitmapFromSvg(Icons.Cube_16x_svg);
            windowPanel1.SectionHeader = "VideoBatch";
        }

        private void BtnMaximize_Click(object? sender, EventArgs e)
        {
            _restoreSize = ClientSize;
            WindowState = (WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal);
            AdjustForm();
        }
        #endregion

        #region Min/Max/Restore for catching resize events to adjust form
        private void BtnMin_Click(object? sender, EventArgs e)
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

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the DoubleClick event from the ProjectTree control.
        /// </summary>
        private async void ProjectTree_DoubleClick(object? sender, EventArgs e)
        {
            // Sender is the AcrylicTreeView within ProjectTree
            var treeView = sender as AcrylicTreeView;
            if (treeView == null)
            {
                _logger.LogWarning("ProjectTree_DoubleClick sender was not an AcrylicTreeView.");
                return;
            }

            var selectedNode = treeView.SelectedNodes?.FirstOrDefault();
            if (selectedNode == null)
            {
                _logger.LogTrace("No node selected on ProjectTree DoubleClick.");
                return;
            }

            // Check if the selected node is our custom TreeItem and has Primitive data in its Tag
            if (selectedNode?.Tag is Primitive primitive)
            {
                _logger.LogInformation("ProjectTree node double-clicked. Primitive ID: {PrimitiveId}, Name: {PrimitiveName}", primitive.ID, primitive.Name);
                try
                {
                    // Use the factory to create the WorkArea
                    WorkArea workArea = await _workAreaFactory.CreateAsync(primitive.ID);

                    // Show the WorkArea in the DockPanel
                    // Ensure _dockPanel is the correct instance variable for your DockPanel
                    if (this.DockPanel != null)
                    {
                         this.DockPanel.AddContent(workArea);
                         _logger.LogDebug("WorkArea for {PrimitiveName} added to DockPanel.", primitive.Name);
                    }
                    else
                    {
                        _logger.LogError("this.DockPanel (designer instance) is null. Cannot display WorkArea.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating or showing WorkArea for Primitive ID {PrimitiveId}.", primitive.ID);
                    // Optionally show an error message to the user
                    MessageBox.Show($"Failed to open item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                 // Log if the selected node wasn't a TreeItem or didn't have a Primitive in Tag
                 _logger.LogWarning("Selected node \"{NodeText}\" is not a TreeItem with a Primitive Tag.", selectedNode.Text);
            }
        }
    }
}
