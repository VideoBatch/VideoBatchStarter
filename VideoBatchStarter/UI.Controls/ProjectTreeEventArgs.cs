namespace VideoBatch.UI.Controls
{
    public class ProjectTreeEventArgs : EventArgs
    {
        public TreeItem Item { get; set; }

        public ProjectTreeEventArgs(TreeItem item)
        {
            Item = item;
        }
    }
}