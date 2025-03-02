using AcrylicUI.Controls;
using AcrylicUI.Docking;
using AcrylicUI.Forms;
using VideoBatch.UI.Controls;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using VideoBatch.Model;
using VideoBatch.UI.Forms;

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
        private TreeNode? _clickedNode;

        public ProjectTree(ILogger<ProjectTree> logger, IProjectServices projectServices)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            
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
        public AcrylicTreeNode QuickJob
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
            selectedNode.Nodes.Add(newNode);
            tvProjectTree.SelectNode(newNode);
            newNode.EnsureVisible();
        }

        // AddTask
        public void AddTask(TreeItem newNode, Project parent, Job j)
        {
            if (j is not null)
            {

                var selectedNode = FindJobNode(parent, j);
                selectedNode.Nodes.Add(newNode);
                tvProjectTree.SelectNode(newNode);
                newNode.EnsureVisible();
            }
        }


        public void AddJob(TreeItem newNode, Project p)
        {
            if (p is not null)
            {
                var selectedNode = FindProjectNode(p);
                selectedNode.Nodes.Add(newNode);
                tvProjectTree.SelectNode(newNode);
                newNode.EnsureVisible();
            }
        }


        public async void BuildTreeView()
        {
            Team team = await _projectServices.GetTeamAsync();
            _logger.LogInformation($"BuildTreeView: Team has {team.Projects.Count} Projects");
            TreeItem node = new(team);
            throw new NotImplementedException("Add Icons to TreeView");
            //node.SvgIcon = ProjectExplorer.Log_16x_svg;
            tvProjectTree.Nodes.Add(node);

            PopulateTreeRecursive(team, node);
            node.Nodes.FirstOrDefault()?.EnsureVisible();
        }

        public TreeItem FindProjectNode(Project p)
        {
            var teamPath = FindTeamPath();
            var fullpath = $"{teamPath}\\{p.Name}";
            return (TreeItem)tvProjectTree.FindNode(fullpath);
        }

        public TreeItem FindJobNode(Project p, Job j)
        {
            var teamPath = FindTeamPath();
            var fullpath = $"{teamPath}\\{p.Name}\\{j.Name}";
            return (TreeItem)tvProjectTree.FindNode(fullpath);
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

        private void _mnuJobNodeRename_Click(object sender, EventArgs e)
        {
            var item = _clickedNode;

            if (_clickedNode is not null && item.Primitive is Job j)
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

        private async void _mnuJobNodeDelete_Click(object sender, EventArgs e)
        {
            var item = _clickedNode;

            // close Window First.


            if (_clickedNode is not null)
            {
                if (item.Primitive is Job j)

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
                        var parent = item.ParentNode;
                        parent.Nodes.Remove(item);
                        await _projectServices.DeleteJobAsync(j);

                        var args = new ProjectTreeEventArgs(item);
                        OnNodeDeleted?.Invoke(sender, args);
                    }
                }

                if (item.Primitive is JobTask t)

                {
                    var result = AcrylicMessageBox.ShowWarning(
                        $"Are Your Sure you want to DELETE {Environment.NewLine}'{t.Name}' ?",
                        $"Delete Task?",
                        AcrylicDialogButton.YesNoCancel
                        );
                    if (result == DialogResult.Yes)
                    {
                        //delete Node                    
                        var parent = item.ParentNode;
                        parent.Nodes.Remove(item);
                        await _projectServices.DeleteTaskAsync(t);

                        var args = new ProjectTreeEventArgs(item);
                        OnNodeDeleted?.Invoke(sender, args);
                    }
                }
            }
            _clickedNode = null;
        }


        private void TvProjectTree_SelectedNodesChanged(object sender, EventArgs e)
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


        private void RightMouseClick(object sender, MouseEventArgs e)
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
