namespace Crypt
{
    partial class FileFolderExplorerForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileFolderExplorerForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.trwFileExplorer = new Crypt.TreeViewMS();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddFilesFolders = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trwFileExplorer
            // 
            this.trwFileExplorer.ImageIndex = 0;
            this.trwFileExplorer.ImageList = this.imageList1;
            this.trwFileExplorer.Location = new System.Drawing.Point(11, 12);
            this.trwFileExplorer.Name = "trwFileExplorer";
            this.trwFileExplorer.SelectedImageIndex = 0;
            this.trwFileExplorer.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("trwFileExplorer.SelectedNodes")));
            this.trwFileExplorer.Size = new System.Drawing.Size(642, 341);
            this.trwFileExplorer.TabIndex = 0;
            this.trwFileExplorer.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.trwFileExplorer_AfterCheck);
            this.trwFileExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trwFileExplorer_BeforeExpand);
            this.trwFileExplorer.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.trwFileExplorer_AfterExpand);
            this.trwFileExplorer.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.trwFileExplorer_AfterCollapse);
            this.trwFileExplorer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trwFileExplorer_KeyDown);
            // 
            // btnAddFilesFolders
            // 
            this.btnAddFilesFolders.Image = global::Crypt.Properties.Resources.Plus__3D__small;
            this.btnAddFilesFolders.Location = new System.Drawing.Point(278, 364);
            this.btnAddFilesFolders.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAddFilesFolders.Name = "btnAddFilesFolders";
            this.btnAddFilesFolders.Size = new System.Drawing.Size(120, 98);
            this.btnAddFilesFolders.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnAddFilesFolders, "Add Files & Folders");
            this.btnAddFilesFolders.UseVisualStyleBackColor = true;
            this.btnAddFilesFolders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFilesFolders.FlatAppearance.BorderSize = 1;
            this.btnAddFilesFolders.Click += new System.EventHandler(this.btnAddFilesFolders_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::Crypt.Properties.Resources.X__3D__small_2;
            this.btnCancel.Location = new System.Drawing.Point(402, 364);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 98);
            this.btnCancel.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnCancel, "Cancel");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 1;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::Crypt.Properties.Resources.Minus__3D__small_2;
            this.btnClear.Location = new System.Drawing.Point(199, 364);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(74, 98);
            this.btnClear.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnClear, "Clear");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.FlatAppearance.BorderSize = 1;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FileFolderExplorerForm
            // 
            this.AcceptButton = this.btnAddFilesFolders;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(662, 472);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnAddFilesFolders);
            this.Controls.Add(this.trwFileExplorer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FileFolderExplorerForm";
            this.Text = "Add Files and Folders";
            this.Load += new System.EventHandler(this.OpenFileFolderExplorer_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private TreeViewMS trwFileExplorer;
        private System.Windows.Forms.Button btnAddFilesFolders;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

