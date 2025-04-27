// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Sep 2021
using System.Windows.Forms;

namespace VideoBatch.UI.Controls
{
    partial class ProjectTree
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tvProjectTree = new AcrylicUI.Controls.AcrylicTreeView();
            SuspendLayout();
            // 
            // tvProjectTree
            // 
            tvProjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            tvProjectTree.Indent = 12;
            tvProjectTree.ItemHeight = 22;
            tvProjectTree.Location = new Point(0, 25);
            tvProjectTree.MaxDragChange = 22;
            tvProjectTree.Name = "tvProjectTree";
            tvProjectTree.ShowIcons = true;
            tvProjectTree.Size = new System.Drawing.Size(150, 438);
            tvProjectTree.TabIndex = 1;
            tvProjectTree.Text = "tvProjectTree";
            // 
            // ProjectTree
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tvProjectTree);
            DefaultDockArea = AcrylicUI.Docking.DockArea.Left;
            DockText = "Projects";
            Name = "ProjectTree";
            Size = new System.Drawing.Size(150, 463);
            ResumeLayout(false);

        }

        #endregion
        private AcrylicUI.Controls.AcrylicTreeView tvProjectTree;

    }
}
