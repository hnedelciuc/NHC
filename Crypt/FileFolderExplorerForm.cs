using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace Crypt
{
    internal partial class FileFolderExplorerForm : Form
    {
       // Initialize FileExplorer class
        FileFolderExplorer fe = new FileFolderExplorer();
        internal FileFolderExplorerForm()
        {
            InitializeComponent();
            SetSelectionSettings();
            SetButtonSettings();
            SetWindowColor();
            AdjustDisplayScaling();
        }

        internal ArrayList selectedNodes = new ArrayList();

        private void trwFileExplorer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Text == "")
            {
                TreeNode node = fe.EnumerateDirectory(e.Node);
            }
            if (e.Node.Checked)
            {
                HelperService.CheckAllNodes(e.Node.Nodes, true);
            }
        }

        private void trwFileExplorer_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 2) // is folder
            {
                e.Node.ImageIndex = 3;
                e.Node.SelectedImageIndex = 3;
            }
        }

        private void trwFileExplorer_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 3) // is folder
            {
                e.Node.ImageIndex = 2;
                e.Node.SelectedImageIndex = 2;
            }
        }

        private void trwFileExplorer_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (HelperService.preventCheck)
                return;

            if (e.Node.Checked)
            {
                HelperService.CheckAllNodes(e.Node.Nodes, true);
            }
            else
            {
                HelperService.CheckAllNodes(e.Node.Nodes, false);
            }
            if (e.Node.Parent != null)
                HelperService.CheckParentNodes(e.Node.Parent.Nodes);
        }

        private void OpenFileFolderExplorer_Load(object sender, EventArgs e)
        {
            // Create file tree            
            fe.CreateTree(this.trwFileExplorer);
            this.trwFileExplorer.TreeViewNodeSorter = new HelperService.NodeSorter(true);
            this.trwFileExplorer.Sort();
        }

        private void trwFileExplorer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                Cursor.Current = Cursors.WaitCursor;
                trwFileExplorer.BeginUpdate();
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                {
                    HelperService.CheckAllNodes(trwFileExplorer.Nodes, true);
                }
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                {
                    ArrayList allNodes = new ArrayList();
                    HelperService.SelectAllNodes(trwFileExplorer.Nodes, true, ref allNodes);
                    trwFileExplorer.SelectedNodes = allNodes;
                }
                trwFileExplorer.EndUpdate();
                Cursor.Current = Cursors.Default;
            }

            if (e.Control && e.KeyCode == Keys.U)
            {
                Cursor.Current = Cursors.WaitCursor;
                trwFileExplorer.BeginUpdate();
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                {
                    HelperService.CheckAllNodes(trwFileExplorer.Nodes, false);
                }
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                {
                    ArrayList allNodes = new ArrayList();
                    HelperService.SelectAllNodes(trwFileExplorer.Nodes, false, ref allNodes);
                    trwFileExplorer.SelectedNodes = allNodes;
                }
                trwFileExplorer.EndUpdate();
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnAddFilesFolders_Click(object sender, EventArgs e)
        {
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
            {
                HelperService.GetCheckedNodes(trwFileExplorer.Nodes, ref selectedNodes);
            }
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
            {
                selectedNodes = trwFileExplorer.SelectedNodes;
            }
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            selectedNodes = new ArrayList();
            trwFileExplorer.CollapseAll();
            trwFileExplorer.SelectedNodes = new ArrayList();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            selectedNodes = new ArrayList();
            this.Close();
        }

        private void SetButtonSettings()
        {
            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsOnly)
            {
                btnAddFilesFolders.Image = Properties.Resources.Plus__3D__small;
                btnAddFilesFolders.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnAddFilesFolders.Text = "";
                toolTip1.SetToolTip(btnAddFilesFolders, "Add Files & Folders");
                btnClear.Image = Properties.Resources.Minus__3D__small_2;
                btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnClear.Text = "";
                toolTip1.SetToolTip(btnClear, "Clear");
                btnCancel.Image = Properties.Resources.X__3D__small_2;
                btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnCancel.Text = "";
                toolTip1.SetToolTip(btnCancel, "Cancel");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.TextOnly)
            {
                btnAddFilesFolders.Image = null;
                btnAddFilesFolders.Text = "Add Files && Folders";
                btnAddFilesFolders.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnAddFilesFolders, "");
                btnClear.Image = null;
                btnClear.Text = "Clear";
                btnClear.TextAlign = ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnClear, "");
                btnCancel.Image = null;
                btnCancel.Text = "Cancel";
                btnCancel.TextAlign = ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnCancel, "");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsAndText)
            {
                btnAddFilesFolders.Image = Properties.Resources.Plus__3D__small_2;
                btnAddFilesFolders.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnAddFilesFolders.Text = "Add Files && Folders";
                btnAddFilesFolders.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnAddFilesFolders, "");
                btnClear.Image = Properties.Resources.Minus__3D__small_2;
                btnClear.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnClear.Text = "Clear";
                btnClear.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnClear, "");
                btnCancel.Image = Properties.Resources.X__3D__small_2;
                btnCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnCancel.Text = "Cancel";
                btnCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnCancel, "");
            }
        }

        private void SetSelectionSettings()
        {
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
            {
                trwFileExplorer.CheckBoxes = true;
            }

            if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
            {
                trwFileExplorer.CheckBoxes = false;
            }
        }

        private void SetWindowColor()
        {
            switch (HelperService.windowColorSetting)
            {
                case HelperService.WindowColorSetting.LightSteelBlue:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
                case HelperService.WindowColorSetting.DarkGray:
                    this.BackColor = System.Drawing.Color.DarkGray;
                    break;
                case HelperService.WindowColorSetting.White:
                    this.BackColor = System.Drawing.Color.White;
                    break;
                default:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
            }
        }

        private void AdjustDisplayScaling()
        {
            btnAddFilesFolders.Image = HelperService.ResizeImagePercent(btnAddFilesFolders.Image, HelperService.scaling);
            btnCancel.Image = HelperService.ResizeImagePercent(btnCancel.Image, HelperService.scaling);
            btnClear.Image = HelperService.ResizeImagePercent(btnClear.Image, HelperService.scaling);

            HelperService.InitializeImageList(imageList1);
        }
    }
}