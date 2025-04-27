using AcrylicUI.Controls;
using VideoBatch.Model;

namespace VideoBatch.UI.Controls
{
    public class TreeItem : AcrylicTreeNode
    {
        public Primitive? Primitive { get; set; } 

        protected TreeItem() { }

        public TreeItem(Primitive p) : this()
        {
            Primitive = p;
            Text = p.Name;

            // TODO: Set Icons based on Type
            // 
        }
    }
}