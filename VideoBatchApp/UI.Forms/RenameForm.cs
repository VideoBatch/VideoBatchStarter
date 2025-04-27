// Copyright (C) ColhounTech Limited. All rights Reserved
// Author: Micheal Colhoun
// Date: Sep 2021

using AcrylicUI.Forms;
using System.ComponentModel;

namespace VideoBatch.UI.Forms
{
    public partial class RenameForm : AcrylicDialog
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NewName
        {
            get { return txtName.Text; }

            set { txtName.Text = value; }
        }
        public RenameForm()
        {
            InitializeComponent();
            IsAcrylic = false;
            Text = "Rename";            
        }

        private void RenameForm_Load(object sender, System.EventArgs e)
        {
            txtName.Focus();
        }
    }
}
