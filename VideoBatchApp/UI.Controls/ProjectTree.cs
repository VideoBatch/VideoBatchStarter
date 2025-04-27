using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Forms;
using VideoBatch.UI.Controls;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using VideoBatch.Model;
using VideoBatch.UI.Forms;
using VideoBatch.Services;

namespace VideoBatch.UI.Controls
{
    public partial class ProjectTree : ToolWindow
    {
        #region public events
        public new event EventHandler? DoubleClick; // Node Double Clicked
        public event ProjectTreeEventHandler? OnNodeDeleted; // Node Deleted
        public event ProjectTreeEventHandler? SelectedNodesChanged; // for Status

        public ObservableCollection<String> SelectedNodesList = [];

        #endregion

        #region Constructors

        private readonly ILogger<ProjectTree> _logger;
        private readonly IProjectServices _projectServices;
        private readonly IDataService _dataService;
        private TreeItem? _clickedNode;

        public ProjectTree(ILogger<ProjectTree> logger, IProjectServices projectServices, IDataService dataService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            
            InitializeComponent();
            HookEvents();
            SetupContextMenu();
            
            Text = "Project Explorer";
            DockText = "Project Explorer";
            DefaultDockArea = DockArea.Left;
        }

        private void HookEvents()
        {
            tvProjectTree.MouseClick += RightMouseClick;
            tvProjectTree.DoubleClick += (sender, e) => DoubleClick?.Invoke(sender, e);
            tvProjectTree.SelectedNodesChanged += TvProjectTree_SelectedNodesChanged;
        }

        #endregion

        #region public Methods
        public ObservableCollection<AcrylicTreeNode> SelectedNodes
        {
            get { return tvProjectTree.SelectedNodes; }
        }
        public void SelectNode(TreeItem node)
        {
            tvProjectTree.SelectNode((AcrylicTreeNode)node);
        }
        public AcrylicTreeNode? QuickJob
        {
            get
            {
                var teamPath = FindTeamPath();
                var fullpath = $"{teamPath}\\Quick Jobs";
                return tvProjectTree.FindNode(fullpath);
            }
        }
        public void Add(TreeItem newNode)
        {
            var selectedNode = tvProjectTree.SelectedNodes.FirstOrDefault();
            if (selectedNode != null)
            {
                selectedNode.Nodes.Add(newNode);
                tvProjectTree.SelectNode(newNode);
                newNode.EnsureVisible();
            }
        }

        // AddTask
        public void AddTask(TreeItem newNode, Project parent, Job j)
        {
            if (j is not null)
            {
                var selectedNode = FindJobNode(parent, j);
                if (selectedNode != null)
                {
                    selectedNode.Nodes.Add(newNode);
                    tvProjectTree.SelectNode(newNode);
                    newNode.EnsureVisible();
                }
            }
        }

        public void AddJob(TreeItem newNode, Project p)
        {
            if (p is not null)
            {
                var selectedNode = FindProjectNode(p);
                if (selectedNode != null)
                {
                    selectedNode.Nodes.Add(newNode);
                    tvProjectTree.SelectNode(newNode);
                    newNode.EnsureVisible();
                }
            }
        }

        /// <summary>
        /// Loads data using IDataService and populates the TreeView.
        /// This should be called after data has been loaded into IDataService (e.g., via LoadDataAsync).
        /// </summary>
        public async void LoadAndPopulateTree()
        {
            _logger.LogInformation("Attempting to populate Project Tree from cached data service data.");
            tvProjectTree.BeginUpdate();
            tvProjectTree.Nodes.Clear();

            try
            {
                // Get the cached Account object from the data service
                Account account = await _dataService.GetAccountAsync(); 
                _logger.LogDebug("Retrieved account '{AccountName}' from data service.", account.Name);

                // Create root node for the account (optional, could start with Teams)
                // TreeItem accountNode = new TreeItem(account);
                // tvProjectTree.Nodes.Add(accountNode);

                if (account.Teams == null || !account.Teams.Any())
                {
                    _logger.LogWarning("Loaded account '{AccountName}' has no Teams to display.", account.Name);
                    tvProjectTree.Nodes.Add(new AcrylicTreeNode("No teams found in project.") { Enabled = false });
                    return; // Exit after clearing
                }

                // Iterate through Teams
                foreach (var team in account.Teams)
                {
                    TreeItem teamNode = new TreeItem(team);
                    tvProjectTree.Nodes.Add(teamNode); // Add Teams directly to root
                    _logger.LogTrace("Added Team node: {TeamName}", team.Name);

                    // Iterate through Projects in the Team
                    if (team.Projects != null)
                    {
                        foreach (var project in team.Projects)
                        {
                            TreeItem projectNode = new TreeItem(project);
                            teamNode.Nodes.Add(projectNode);
                            _logger.LogTrace("  Added Project node: {ProjectName}", project.Name);

                            // Iterate through Jobs in the Project
                            if (project.Jobs != null)
                            {
                                foreach (var job in project.Jobs)
                                {
                                    TreeItem jobNode = new TreeItem(job);
                                    projectNode.Nodes.Add(jobNode);
                                    _logger.LogTrace("    Added Job node: {JobName}", job.Name);

                                    // Iterate through Tasks in the Job
                                    if (job.Tasks != null)
                                    {
                                        foreach (var task in job.Tasks)
                                        {
                                            TreeItem taskNode = new TreeItem(task);
                                            jobNode.Nodes.Add(taskNode);
                                             _logger.LogTrace("      Added Task node: {TaskName}", task.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                } 

                // Expand the first level (Team nodes)
                if (tvProjectTree.Nodes.Any())
                {
                    foreach(var node in tvProjectTree.Nodes)
                    {
                        node.Expand();
                    }
                    tvProjectTree.Nodes[0].EnsureVisible(); 
                }
                _logger.LogInformation("Project Tree populated successfully.");

            }
            catch (InvalidOperationException ioEx)
            {
                 _logger.LogError(ioEx, "Failed to populate Project Tree because data service was not loaded.");
                 tvProjectTree.Nodes.Add(new AcrylicTreeNode("Error: Project data not loaded.") { ForeColor = Color.Red });
                 // Optionally show a message box or update status bar from the main form
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An unexpected error occurred while populating the Project Tree.");
                 tvProjectTree.Nodes.Add(new AcrylicTreeNode("Error populating tree.") { ForeColor = Color.Red });
            }
            finally
            {
                tvProjectTree.EndUpdate();
            }
        }

        public TreeItem? FindProjectNode(Project p)
        {
            var teamPath = FindTeamPath();
            var fullpath = $"{teamPath}\\{p.Name}";
            return (TreeItem?)tvProjectTree.FindNode(fullpath);
        }

        public TreeItem? FindJobNode(Project p, Job j)
        {
            var teamPath = FindTeamPath();
            var fullpath = $"{teamPath}\\{p.Name}\\{j.Name}";
            return (TreeItem?)tvProjectTree.FindNode(fullpath);
        }

        /// <summary>
        /// var string = $"{}"Mícheál's Team\\Quick Jobs"";   
        /// </summary>
        /// <returns></returns>
        public string FindTeamPath()
        {
            return tvProjectTree.Nodes[0].Text;
        }

        #endregion

        #region private Methods

        private void PopulateTreeRecursive(Primitive target, AcrylicTreeNode node)
        {
            foreach (Primitive i in target.GetEnumerator())
            {
                TreeItem treeItem = new(i);
                node.Nodes.Add(treeItem);
                var p = node.Nodes.Last();
                PopulateTreeRecursive(i, p);
            }
        }

        private void SetupContextMenu()
        {
            _mnu.Items.Add(_mnuJobNodeDelete);
            _mnu.Items.Add(_mnuJobNodeRename);
            _mnuJobNodeDelete.Click += _mnuJobNodeDelete_Click;
            _mnuJobNodeRename.Click += _mnuJobNodeRename_Click;
        }

        private void _mnuJobNodeRename_Click(object? sender, EventArgs e)
        {
            if (_clickedNode is not null && _clickedNode.Primitive is Job j)
            {
                var rename = _clickedNode.Text;
                var dialog = new RenameForm { DialogButtons = AcrylicDialogButton.OkCancel };
                dialog.NewName = _clickedNode.Text;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _clickedNode.Text = dialog.NewName;
                    _clickedNode.Primitive.Name = dialog.NewName;
                    _projectServices.Update();
                    // TODO: Update Database
                }
            }
            _clickedNode = null;
        }

        private async void _mnuJobNodeDelete_Click(object? sender, EventArgs e)
        {
            if (_clickedNode is not null)
            {
                if (_clickedNode.Primitive is Job j)
                {
                    if (j.Tasks?.Count > 0)
                    {
                        AcrylicMessageBox.ShowError($"Can't Delete a Job with Child Tasks.{Environment.NewLine}Remove all Tasks First", "Job has existing Tasks");
                        return;
                    }
                    var result = AcrylicMessageBox.ShowWarning(
                        $"Are Your Sure you want to DELETE {Environment.NewLine}'{j.Name}' ?",
                        $"Delete Job?",
                        AcrylicDialogButton.YesNoCancel
                        );
                    if (result == DialogResult.Yes)
                    {
                        //delete Node                    
                        var parent = _clickedNode.ParentNode;
                        if (parent != null)
                        {
                            parent.Nodes.Remove(_clickedNode);
                            await _projectServices.DeleteJobAsync(j);

                            var args = new ProjectTreeEventArgs(_clickedNode);
                            OnNodeDeleted?.Invoke(this, args);
                        }
                    }
                }

                if (_clickedNode.Primitive is JobTask t)
                {
                    var result = AcrylicMessageBox.ShowWarning(
                        $"Are Your Sure you want to DELETE {Environment.NewLine}'{t.Name}' ?",
                        $"Delete Task?",
                        AcrylicDialogButton.YesNoCancel
                        );
                    if (result == DialogResult.Yes)
                    {
                        //delete Node                    
                        var parent = _clickedNode.ParentNode;
                        if (parent != null)
                        {
                            parent.Nodes.Remove(_clickedNode);
                            await _projectServices.DeleteTaskAsync(t);

                            var args = new ProjectTreeEventArgs(_clickedNode);
                            OnNodeDeleted?.Invoke(this, args);
                        }
                    }
                }
            }
            _clickedNode = null;
        }

        private void TvProjectTree_SelectedNodesChanged(object? sender, EventArgs e)
        {
            if (tvProjectTree.SelectedNodes.Count > 0)
            {
                Console.WriteLine("Selected Nodes");

                SelectedNodesList.Clear();
                foreach (TreeItem node in tvProjectTree.SelectedNodes)
                {
                    SelectedNodesList.Add(node.Text);
                    Console.WriteLine($"TvProjectTree_SelectedNodesChanged {node.Text} ");
                    SelectedNodesChanged?.Invoke(this, new ProjectTreeEventArgs(node));
                }
            }
        }

        private void RightMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var node = (sender as AcrylicTreeView)?.SelectedNodes?.FirstOrDefault();
                if (node == null || node.IsRoot) return;
                if (node is TreeItem item)
                {
                    if (item.Primitive is Job j)
                    {
                        _clickedNode = item;
                        _mnu.Show(tvProjectTree, e.Location);
                    }
                    if (item.Primitive is JobTask t)
                    {
                        _clickedNode = item;
                        _mnu.Show(tvProjectTree, e.Location);
                    }
                }
            }
        }

        #endregion

        #region Private fields

        private readonly AcrylicContextMenu _mnu = new AcrylicContextMenu();
        private readonly ToolStripMenuItem _mnuJobNodeDelete = new ToolStripMenuItem("Delete");
        private readonly ToolStripMenuItem _mnuJobNodeRename = new ToolStripMenuItem("Rename");

        #endregion
    }
}
