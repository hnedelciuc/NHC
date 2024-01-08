/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Main window / main menu.
/********************************************************************************************************************/

using GracefulDynamicDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Crypt
{
    internal partial class CryptForm : Form
    {
        /********************************************************************************************************************/
        internal CryptForm()
        {
            InitializeComponent();
            GetDisplayScaleFactor();
            SetSelectionSettings();
            SetButtonSettings();
            SetWindowColorSettings();
            AdjustDisplayScaling(false);
            InitializeOtherStuff();
            SetCompressionSettings();
            SetCryptionSettings();
            LoadInputFiles();
        }

        ContextMenu menuTreeView = new ContextMenu();
        MenuItem menuTreeViewItem1 = new MenuItem("Info Regarding Selected Entries");
        MenuItem menuTreeViewItem3 = new MenuItem("Update Archive Contents");
        MenuItem menuTreeViewItem4 = new MenuItem("Add .NHC Archive to Current Archive"); // TO DO: make it "to Current Archive at Selected Node"
        MenuItem menuTreeViewItem5 = new MenuItem("Add Files && Folders to Current Archive"); // TO DO: make it "to Current Archive at Selected Node"
        ContextMenu menuCompressEncryptExtra = new ContextMenu();
        MenuItem menuCompressEncryptExtraItem1 = new MenuItem("Info Regarding Selected Entries");
        MenuItem menuCompressEncryptExtraItem3 = new MenuItem("Update Archive Contents");
        ContextMenu menuUpdateArchive1 = new ContextMenu();
        MenuItem menuUpdateArchive1Item1 = new MenuItem("Add .NHC Archive to Current Archive"); // TO DO: make it "to Current Archive at Selected Node"
        MenuItem menuUpdateArchive1Item3 = new MenuItem("Add Files && Folders to Current Archive"); // TO DO: make it "to Current Archive at Selected Node"
        TreeNode clickedNode = null;
        string prevComboBoxSplitArchiveSizeText = string.Empty;

        /********************************************************************************************************************/

        // Show info regarding files
        private void menuTreeViewItem1_Click(object sender, EventArgs e)
        {
            ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.InfoRegardingSelectedEntries, clickedNode, treeView1);
            frmProcessing.ShowDialog();
        }

        /********************************************************************************************************************/

        // Update archive contents
        private void menuTreeViewItem3_Click(object sender, EventArgs e)
        {
            ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.UpdateArchiveContents, treeView1);
            frmProcessing.ShowDialog();

            var selectedPaths = new string[HelperService.selectedPaths.Count];
            var selectedPathsCorrespondingArchive = new string[selectedPaths.Length];

            try
            {
                for (var i = 0; i < HelperService.selectedPaths.Count; i++)
                {
                    dynamic pathObj = HelperService.selectedPaths[i];
                    selectedPaths[i] = pathObj.relativePath;
                    selectedPathsCorrespondingArchive[i] = pathObj.currentArchiveName;
                }
            }
            catch { }

            string password = String.Empty;
            string keyFileName = String.Empty;
            string outputFileName = String.Empty;
            HelperService.OverwriteFilesSetting overwriteFilesSetting = radioButtonOverwriteFilesAsk.Checked
                ? HelperService.OverwriteFilesSetting.Ask : radioButtonOverwriteFilesYes.Checked
                    ? HelperService.OverwriteFilesSetting.Yes : HelperService.OverwriteFilesSetting.No;

            if (HelperService.importedPaths.Count > 0)
            {
                if (textBoxKeyFile.Enabled == true && textBoxKeyFile.Text.Trim() == String.Empty)
                {
                    MessageBox.Show("Please indicate a cryption key file or choose default built-in key.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (maskedTextBoxPassword.Enabled == true && string.IsNullOrEmpty(maskedTextBoxPassword.Text))
                {
                    MessageBox.Show("Please make sure you have typed a password in order for the extraction to take place.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (textBoxKeyFile.Enabled == true) keyFileName = textBoxKeyFile.Text.Trim();
                    if (maskedTextBoxPassword.Enabled == true) password = maskedTextBoxPassword.Text;

                    InputBoxForm frmInputBox = new InputBoxForm("Update Archive", "Please enter name for the new updated archive:");
                    frmInputBox.ShowDialog();

                    outputFileName = frmInputBox.Value;

                    if (string.IsNullOrWhiteSpace(outputFileName))
                        return;

                    checkIfSplit:
                    long? splitArchiveSize = (long?)ValidateSplitArchiveSize();

                    if (splitArchiveSize == null)
                    {
                        if (!string.IsNullOrWhiteSpace(comboBoxSplitArchiveSize.Text))
                        {
                            var isSplitArch = MessageBox.Show("You have indicated a maximum split archive size, but you have not checked the corresponding \"Split Archive\" checkbox. Do you want this to be a split archive (Yes) or a solid/single archive (No)?",
                                "Is this a split archive or not?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                            if (isSplitArch == DialogResult.Yes)
                            {
                                checkBoxIsSplitArchive.Checked = true;
                                goto checkIfSplit;
                            }
                            else if (isSplitArch == DialogResult.Cancel)
                            {
                                return;
                            }
                            else
                            {
                                checkBoxIsSplitArchive.Checked = false;
                                comboBoxSplitArchiveSize.SelectedIndex = -1;
                            }
                        }
                    }

                    ProgressForm frmProgress = new ProgressForm(HelperService.CryptionOptions.Update, HelperService.cryptionAlgorithm, HelperService.importedPaths, selectedPaths, selectedPathsCorrespondingArchive, frmProcessing.extractAll, overwriteFilesSetting, HelperService.compressionLevel, outputFileName, keyFileName, checkBoxIsSplitArchive.Checked ? (long)splitArchiveSize : 0, password);
                    frmProgress.ShowDialog();

                    HelperService.importedPaths.Clear();

                    HelperService.importedPaths.Add(new DDict(new Dictionary<string, dynamic>()
                    {
                        { "alreadyOpened", false },
                        { "relativePath", Path.GetFileName(outputFileName) },
                        { "fullPath", LongFile.GetWin32LongPath(outputFileName) },
                        { "isDirectory", false },
                        { "keyFilePath", HelperService.keyFileName },
                        { "password", HelperService.pwd },
                        { "cryptionAlgorithm", HelperService.cryptionAlgorithm }
                    }));

                    HelperService.exitOnClose = false;

                    Close();
                }
            }
        }

        /********************************************************************************************************************/

        private void menuTreeViewItem4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Crypt File|*.nhc|All Files|*.*";
            openFileDialog1.Title = "Choose Input Files (archives)";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var paths = openFileDialog1.FileNames.Select(filename => new { alreadyOpened = false, fullPath = filename, relativePath = string.Empty, keyFilePath = textBoxKeyFile.Text, password = maskedTextBoxPassword.Text, cryptionAlgorithm = HelperService.cryptionAlgorithm }).ToList();
                HelperService.importedPaths.AddRange(paths);
                openFileDialog1.FileName = String.Empty;

                HelperService.exitOnClose = false;

                Close();
            }
        }

        /********************************************************************************************************************/

        private void menuTreeViewItem5_Click(object sender, EventArgs e)
        {
            FileFolderExplorerForm frmFileFolderExplorer = new FileFolderExplorerForm();

            frmFileFolderExplorer.ShowDialog();

            ArrayList addedNodes = frmFileFolderExplorer.selectedNodes;

            ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.ImportFilesDirectories_btnBrowseInput_Click, addedNodes);
            frmProcessing.ShowDialog();

            treeView1.BeginUpdate();
            frmProcessing.treeView1.BeginUpdate();
            treeView1.SelectedNodes = frmProcessing.treeView1.SelectedNodes;
            ArrayList nodes = new ArrayList(frmProcessing.treeView1.Nodes);
            foreach (TreeNode node in nodes)
            {
                node.Remove(); // remove from frmProcessing.treeView1
                treeView1.Nodes.Add(node); // add to treeView1
            }
            frmProcessing.treeView1.EndUpdate();
            treeView1.EndUpdate();
        }

        /********************************************************************************************************************/

        private void menuCompressEncryptExtraItem1_Click(object sender, EventArgs e)
        {
            menuTreeViewItem1_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void menuCompressEncryptExtraItem3_Click(object sender, EventArgs e)
        {
            menuTreeViewItem3_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void menuUpdateArchive1Item1_Click(object sender, EventArgs e)
        {
            menuTreeViewItem3_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void menuUpdateArchive1Item2_Click(object sender, EventArgs e)
        {
            menuTreeViewItem4_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void menuUpdateArchive1Item3_Click(object sender, EventArgs e)
        {
            menuTreeViewItem5_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void menuUpdateArchive2Item1_Click(object sender, EventArgs e)
        {
            menuTreeViewItem3_Click(sender, e);
        }

        /********************************************************************************************************************/

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            if (radioButtonCompressEncrypt.Checked == true)
            {
                FileFolderExplorerForm frmFileFolderExplorer = new FileFolderExplorerForm();

                frmFileFolderExplorer.ShowDialog();

                ArrayList addedNodes = frmFileFolderExplorer.selectedNodes;

                ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.ImportFilesDirectories_btnBrowseInput_Click, addedNodes);
                frmProcessing.ShowDialog();

                treeView1.BeginUpdate();
                frmProcessing.treeView1.BeginUpdate();
                treeView1.SelectedNodes = frmProcessing.treeView1.SelectedNodes;
                ArrayList nodes = new ArrayList(frmProcessing.treeView1.Nodes);
                foreach (TreeNode node in nodes)
                {
                    node.Remove(); // remove from frmProcessing.treeView1
                    treeView1.Nodes.Add(node); // add to treeView1
                }
                frmProcessing.treeView1.EndUpdate();
                treeView1.EndUpdate();
            }
            else
            if (radioButtonDecompressDecrypt.Checked == true)
            {
                openFileDialog1.Filter = "Crypt File|*.nhc|All Files|*.*";
                openFileDialog1.Title = "Choose Input Files (archives)";
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var paths = openFileDialog1.FileNames.Select(filename => new { alreadyOpened = false, fullPath = filename, relativePath = string.Empty, keyFilePath = textBoxKeyFile.Text, password = maskedTextBoxPassword.Text, cryptionAlgorithm = HelperService.cryptionAlgorithm }).ToList();
                    HelperService.importedPaths = new ArrayList(paths);
                    openFileDialog1.FileName = String.Empty;

                    HelperService.exitOnClose = false;

                    Close();
                }
            }
        }

        /********************************************************************************************************************/

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /********************************************************************************************************************/

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (paths != null && paths.Length != 0)
            {
                if (radioButtonCompressEncrypt.Checked)
                {
                    ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.ImportFilesDirectories_treeView1_DragDrop, paths);
                    frmProcessing.ShowDialog();

                    treeView1.BeginUpdate();
                    frmProcessing.treeView1.BeginUpdate();
                    treeView1.SelectedNodes = frmProcessing.treeView1.SelectedNodes;
                    ArrayList nodes = new ArrayList(frmProcessing.treeView1.Nodes);
                    foreach (TreeNode node in nodes)
                    {
                        node.Remove(); // remove from frmProcessing.treeView1
                        treeView1.Nodes.Add(node); // add to treeView1
                    }
                    frmProcessing.treeView1.EndUpdate();
                    treeView1.EndUpdate();
                }
                else // if decompress...
                {
                    HelperService.importedPaths = new ArrayList(paths.Select(filename => new { alreadyOpened = false, fullPath = filename, relativePath = string.Empty, keyFilePath = textBoxKeyFile.Text, password = maskedTextBoxPassword.Text, cryptionAlgorithm = HelperService.cryptionAlgorithm }).ToArray());

                    HelperService.exitOnClose = false;

                    Close();
                }
            }
        }

        /********************************************************************************************************************/

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            treeView1.BeginUpdate();
            ArrayList selectedPaths = new ArrayList();
            ArrayList selectedNodes = new ArrayList();
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
            {
                HelperService.GetCheckedNodes(treeView1.Nodes, ref selectedNodes);
            }
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
            {
                selectedNodes = treeView1.SelectedNodes;
            }
            foreach (TreeNode node in selectedNodes)
            {
                selectedPaths.Add(node.FullPath);
            }
            treeView1.SelectedNodes = new ArrayList();
            HelperService.RemoveSelectedNodes(treeView1.Nodes, selectedPaths);
            HelperService.RemoveSelectedPaths(selectedPaths);
            treeView1.EndUpdate();
            Cursor.Current = Cursors.Default;
        }

        /********************************************************************************************************************/

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Cursor.Current = Cursors.WaitCursor;
                treeView1.BeginUpdate();
                ArrayList selectedPaths = new ArrayList();
                ArrayList selectedNodes = new ArrayList();
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                {
                    HelperService.GetCheckedNodes(treeView1.Nodes, ref selectedNodes);
                }
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                {
                    selectedNodes = treeView1.SelectedNodes;
                }
                foreach (TreeNode node in selectedNodes)
                {
                    selectedPaths.Add(node.FullPath);
                }
                treeView1.SelectedNodes = new ArrayList();
                HelperService.RemoveSelectedNodes(treeView1.Nodes, selectedPaths);
                HelperService.RemoveSelectedPaths(selectedPaths);
                treeView1.EndUpdate();
                Cursor.Current = Cursors.Default;
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                Cursor.Current = Cursors.WaitCursor;
                treeView1.BeginUpdate();
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                {
                    removeAfterCheckEvent();
                    HelperService.CheckAllNodes(treeView1.Nodes, true);
                    addAfterCheckEvent();
                }
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                {
                    ArrayList allNodes = new ArrayList();
                    HelperService.SelectAllNodes(treeView1.Nodes, true, ref allNodes);
                    treeView1.SelectedNodes = allNodes;
                }
                treeView1.EndUpdate();
                Cursor.Current = Cursors.Default;
            }

            if (e.Control && e.KeyCode == Keys.U)
            {
                Cursor.Current = Cursors.WaitCursor;
                treeView1.BeginUpdate();
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                {
                    removeAfterCheckEvent();
                    HelperService.CheckAllNodes(treeView1.Nodes, false);
                    addAfterCheckEvent();
                }
                if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                {
                    ArrayList allNodes = new ArrayList();
                    HelperService.SelectAllNodes(treeView1.Nodes, false, ref allNodes);
                    treeView1.SelectedNodes = allNodes;
                }
                treeView1.EndUpdate();
                Cursor.Current = Cursors.Default;
            }
        }

        /********************************************************************************************************************/

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            removeAfterCheckEvent();
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
            addAfterCheckEvent();
        }

        /********************************************************************************************************************/

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView1.CheckBoxes)
            {
                removeAfterCheckEvent();
                if (e.Node.Checked)
                {
                    HelperService.CheckAllNodes(e.Node.Nodes, true);
                }
                addAfterCheckEvent();
            }
        }

        /********************************************************************************************************************/

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 2) // is FolderClosed
            {
                e.Node.ImageIndex = 3;  // FolderOpened
                e.Node.SelectedImageIndex = 3;
            }
        }

        /********************************************************************************************************************/

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ImageIndex == 3) // is FolderOpened
            {
                e.Node.ImageIndex = 2;  // FolderClosed
                e.Node.SelectedImageIndex = 2;
            }
        }

        /********************************************************************************************************************/

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                clickedNode = e.Node;
                menuTreeView.Show(treeView1, e.Location);
            }
        }

        /********************************************************************************************************************/

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if (radioButtonCompressEncrypt.Checked == true)
            {
                saveFileDialog1.Filter = "Crypt File|*.nhc|Other File|*.*";
                saveFileDialog1.Title = "Choose Output File (archive)";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBoxOutputFile.Text = saveFileDialog1.FileName;
                    HelperService.output = textBoxOutputFile.Text;
                }
            }
            else
            if (radioButtonDecompressDecrypt.Checked == true)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBoxOutputFile.Text = folderBrowserDialog1.SelectedPath;
                    HelperService.output = textBoxOutputFile.Text;
                }
            }
        }

        /********************************************************************************************************************/

        private void textBoxOutputFile_Leave(object sender, System.EventArgs e)
        {
            if (!LongFile.Exists(textBoxOutputFile.Text)
            || (MessageBox.Show("Output file " + textBoxOutputFile.Text + " already exists. Do you want to replace it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
            {
                saveFileDialog1.FileName = textBoxOutputFile.Text;
            }
            else
            {
                textBoxOutputFile.Text = String.Empty;
                HelperService.output = String.Empty;
            }
        }

        /********************************************************************************************************************/

        private void textBoxOutputFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /********************************************************************************************************************/

        private void textBoxOutputFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (paths != null && paths.Length != 0)
            {
                if (radioButtonCompressEncrypt.Checked)
                    if (!LongDirectory.Exists(paths[0]))
                    {
                        if (!LongFile.Exists(paths[0])
                        || (MessageBox.Show("Output file " + paths[0] + " already exists. Do you want to replace it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
                        {
                            textBoxOutputFile.Text = paths[0];
                            saveFileDialog1.FileName = paths[0];
                            HelperService.output = paths[0];
                        }
                        else
                        {
                            textBoxOutputFile.Text = String.Empty;
                            HelperService.output = String.Empty;
                        }
                    }

                if (radioButtonDecompressDecrypt.Checked)
                    if (LongDirectory.Exists(paths[0]))
                    {
                        textBoxOutputFile.Text = paths[0];
                        openFileDialog1.FileName = paths[0];
                        HelperService.output = paths[0];
                    }
            }
        }

        /********************************************************************************************************************/

        private void btnBrowseKey_Click(object sender, EventArgs e)
        {
            if (radioButtonUseExistingKey.Checked == true)
            {
                openFileDialog2.Filter = "Cryption Key File|*.nhk|Other File|*.*";
                openFileDialog2.Title = "Choose Cryption Key Input File";

                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    textBoxKeyFile.Text = openFileDialog2.FileName;
                }
            }
            else
                if (radioButtonGenerateNewKey.Checked == true)
            {
                saveFileDialog2.Filter = "Cryption Key File|*.nhk|Other File|*.*";
                saveFileDialog2.Title = "Choose Cryption Key Output File";
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    textBoxKeyFile.Text = saveFileDialog2.FileName;
                }
            }
        }

        /********************************************************************************************************************/

        private void textBoxKeyFile_TextChanged(object sender, EventArgs e)
        {
            openFileDialog2.FileName = textBoxKeyFile.Text;
            saveFileDialog2.FileName = textBoxKeyFile.Text;
            HelperService.keyFileName = textBoxKeyFile.Text;
        }

        /********************************************************************************************************************/

        private void textBoxKeyFile_Leave(object sender, System.EventArgs e)
        {
            if (radioButtonGenerateNewKey.Checked == true)
            {
                if (textBoxKeyFile.Text != String.Empty)
                {
                    if (LongDirectory.Exists(textBoxKeyFile.Text))
                    {
                        MessageBox.Show("A folder with this name already exists. Please choose a different output file name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBoxKeyFile.Text = String.Empty;
                    }
                    else if (LongFile.Exists(textBoxKeyFile.Text))
                    {
                        if (MessageBox.Show("Output file " + textBoxKeyFile.Text + " already exists. Do you want to replace it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            saveFileDialog1.FileName = textBoxKeyFile.Text;
                        }
                        else textBoxKeyFile.Text = String.Empty;
                    }
                }
            }
        }

        /********************************************************************************************************************/

        private void textBoxKeyFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /********************************************************************************************************************/

        private void textBoxKeyFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (paths != null && paths.Length != 0)
            {
                if (!LongDirectory.Exists(paths[0]))
                {
                    textBoxKeyFile.Text = paths[0];
                    openFileDialog2.FileName = paths[0];
                    saveFileDialog2.FileName = paths[0];
                }
            }
        }

        /********************************************************************************************************************/
        
        private static bool showPassword = false;
        private void btnShowHidePassword_Click(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            if (showPassword)
            {
                if (HelperService.buttonSetting == 0)
                    toolTip1.SetToolTip(btnShowHidePassword, "Hide");
                else btnShowHidePassword.Text = "Hide";
                maskedTextBoxPassword.UseSystemPasswordChar = false;
            }
            else
            {
                if (HelperService.buttonSetting == 0)
                    toolTip1.SetToolTip(btnShowHidePassword, "Show");
                else btnShowHidePassword.Text = "Show";
                maskedTextBoxPassword.UseSystemPasswordChar = true;
            }
        }

        /********************************************************************************************************************/

        private void maskedTextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            HelperService.pwd = maskedTextBoxPassword.Text;
        }

        /********************************************************************************************************************/

        private void radioButtonCompressionHigh_CheckedChanged(object sender, EventArgs e)
        {
            HelperService.compressionLevel = radioButtonCompressionHigh.Checked
                ? CompressionLevel.Optimal
                : radioButtonCompressionLow.Checked
                    ? CompressionLevel.Fastest
                    : CompressionLevel.NoCompression;
        }

        /********************************************************************************************************************/

        private void radioButtonCompressionLow_CheckedChanged(object sender, EventArgs e)
        {
            HelperService.compressionLevel = radioButtonCompressionHigh.Checked
                ? CompressionLevel.Optimal
                : radioButtonCompressionLow.Checked
                    ? CompressionLevel.Fastest
                    : CompressionLevel.NoCompression;

        }

        /********************************************************************************************************************/

        private void radioButtonCompressionNone_CheckedChanged(object sender, EventArgs e)
        {
            HelperService.compressionLevel = radioButtonCompressionHigh.Checked
                ? CompressionLevel.Optimal
                : radioButtonCompressionLow.Checked
                    ? CompressionLevel.Fastest
                    : CompressionLevel.NoCompression;
        }

        /********************************************************************************************************************/

        private void radioButtonCompressEncrypt_CheckedChanged(object sender, EventArgs e)
        {
            SetCompressEncryptButtonSetting();

            HelperService.cryptionSetting = HelperService.CryptionOptions.Encrypt;
            menuTreeViewItem3.Visible = false;
            menuTreeViewItem4.Visible = false;
            menuCompressEncryptExtraItem3.Visible = false;
            btnUpdateArchive1.Visible = false;
            btnUpdateArchive1.Text = string.Empty;
            radioButtonGenerateNewKey.Enabled = true;
            groupBoxOverwriteFiles.Enabled = false;
            groupBoxChooseCompressionLevel.Enabled = true;
            label1.Text = "Please choose input files to be archived:";
            label2.Text = "Please choose output file (archive):";
            if (isManualCompressEncryptCheckChanged)
            {
                HelperService.importedPaths = new ArrayList();
                HelperService.output = null;
            }
            treeView1.SelectedNodes = new ArrayList();
            treeView1.Nodes.Clear();
            textBoxOutputFile.Text = String.Empty;
            openFileDialog1.FileName = String.Empty;
        }

        /********************************************************************************************************************/

        private void radioButtonDecompressDecrypt_CheckedChanged(object sender, EventArgs e)
        {
            SetCompressEncryptButtonSetting();

            HelperService.cryptionSetting = HelperService.CryptionOptions.Decrypt;
            menuTreeViewItem3.Visible = true;
            menuTreeViewItem4.Visible = true;
            menuTreeViewItem5.Visible = true;
            menuCompressEncryptExtraItem3.Visible = true;
            btnUpdateArchive1.Visible = true;
            btnUpdateArchive1.Text = "▼";
            
            //if (radioButtonGenerateNewKey.Checked == true)
            //{
            //    radioButtonUseExistingKey.Checked = true;
            //}
            //radioButtonGenerateNewKey.Enabled = false;
            
            radioButtonGenerateNewKey.Enabled = true;
            
            //

            groupBoxOverwriteFiles.Enabled = true;
            //groupBoxChooseCompressionLevel.Enabled = false;
            label1.Text = "Please choose input files (archives) for extraction:";
            label2.Text = "Please choose output folder for extracting:";
            if (isManualCompressEncryptCheckChanged)
            {
                HelperService.importedPaths = new ArrayList();
                HelperService.output = null;
                HelperService.compressedFilesSize = 0;
                HelperService.compressedHeadersSize = 0;
                HelperService.uncompressedFilesSize = 0;
                HelperService.archiveFileSize = 0;
            }
            treeView1.SelectedNodes = new ArrayList();
            treeView1.Nodes.Clear();
            textBoxOutputFile.Text = String.Empty;
            saveFileDialog1.FileName = String.Empty;
        }

        /********************************************************************************************************************/

        private void radioButtonUseBuiltInDefault_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = false;
            textBoxKeyFile.Enabled = false;
            btnBrowseKey.Enabled = false;
            maskedTextBoxPassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            btnShowHidePassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            btnGenerateKey.Enabled = false;
            btnCheckKey.Enabled = false;
        }

        /********************************************************************************************************************/

        private void radioButtonUseExistingKey_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = true;
            textBoxKeyFile.Enabled = true;
            btnBrowseKey.Enabled = true;
            btnCheckKey.Enabled = true;
            btnGenerateKey.Enabled = false;
            maskedTextBoxPassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            btnShowHidePassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            label3.Text = "Please choose existing cryption key file:";
        }

        /********************************************************************************************************************/

        private void radioButtonGenerateNewKey_CheckedChanged(object sender, EventArgs e)
        {
            btnGenerateKey.Enabled = true;
            btnCheckKey.Enabled = true;
            label3.Enabled = true;
            textBoxKeyFile.Enabled = true;
            btnBrowseKey.Enabled = true;
            maskedTextBoxPassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            btnShowHidePassword.Enabled = HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword ? true : false;
            label3.Text = "Please choose new cryption key file:";
        }

        /********************************************************************************************************************/

        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            string keyFileName = textBoxKeyFile.Text;

            if (keyFileName == String.Empty)
            {
                MessageBox.Show("Please choose a valid path where to save the Cryption Key File first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            if (CryptionService.GenerateKey(textBoxKeyFile.Text, HelperService.cryptionAlgorithm) == true)
            {
                // Displays the Success MessageBox.
                MessageBox.Show("Random key file has been generated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Displays the Fail MessageBox.
                MessageBox.Show("Random key file has not been generated.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /********************************************************************************************************************/

        private void btnCheckKey_Click(object sender, EventArgs e)
        {
            string keyFileName = textBoxKeyFile.Text;

            if (keyFileName == String.Empty)
            {
                MessageBox.Show("Please choose Cryption Key File first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            if (CryptionService.CheckKey(keyFileName, HelperService.cryptionAlgorithm) == true)
            {
                // Displays the Success MessageBox.
                MessageBox.Show("Cryption Key File is valid.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Displays the Fail MessageBox.
                MessageBox.Show("Cryption Key File is not valid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /********************************************************************************************************************/

        private void btnCompressEncrypt_Click(object sender, EventArgs e)
        {
            long? splitArchiveSize = (long?)ValidateSplitArchiveSize();

            if (radioButtonCompressEncrypt.Checked == true)
            {
                string password = string.Empty;
                string keyFileName = String.Empty;
                string outputFileName = String.Empty;
                var compressionLevel = CompressionLevel.Optimal;

                // If the file names are not an empty string, call corresponding methods for processing.
                if (HelperService.importedPaths.Count > 0 && textBoxOutputFile.Text.Trim() != String.Empty)
                {
                    if (textBoxKeyFile.Enabled == true && textBoxKeyFile.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please indicate a cryption key file or choose default built-in key.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (maskedTextBoxPassword.Enabled == true && string.IsNullOrEmpty(maskedTextBoxPassword.Text))
                    {
                        MessageBox.Show("Please make sure you have typed a password in order for the encryption to take place.",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (radioButtonCompressionHigh.Checked == true)
                        {
                            compressionLevel = CompressionLevel.Optimal;
                        }

                        if (radioButtonCompressionLow.Checked == true)
                        {
                            compressionLevel = CompressionLevel.Fastest;
                        }

                        if (radioButtonCompressionNone.Checked == true)
                        {
                            compressionLevel = CompressionLevel.NoCompression;
                        }

                        if (textBoxKeyFile.Enabled == true) keyFileName = textBoxKeyFile.Text.Trim();
                        if (maskedTextBoxPassword.Enabled == true) password = maskedTextBoxPassword.Text;

                        outputFileName = textBoxOutputFile.Text.Trim();

                    checkIfSplit:
                        if (splitArchiveSize == null)
                        {
                            if (!string.IsNullOrWhiteSpace(comboBoxSplitArchiveSize.Text))
                            {
                                var isSplitArch = MessageBox.Show("You have indicated a maximum split archive size, but you have not checked the corresponding \"Split Archive\" checkbox. Do you want this to be a split archive (Yes) or a solid/single archive (No)?",
                                    "Is this a split archive or not?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                                if (isSplitArch == DialogResult.Yes)
                                {
                                    checkBoxIsSplitArchive.Checked = true;
                                    splitArchiveSize = (long?)ValidateSplitArchiveSize();
                                    goto checkIfSplit;
                                }
                                else if (isSplitArch == DialogResult.Cancel)
                                {
                                    return;
                                }
                                else
                                {
                                    checkBoxIsSplitArchive.Checked = false;
                                    comboBoxSplitArchiveSize.Text = string.Empty;
                                    comboBoxSplitArchiveSize.SelectedIndex = -1;
                                }
                            }
                            else if (checkBoxIsSplitArchive.Checked) { return; }
                        }

                        ProgressForm frmProgress = new ProgressForm(HelperService.CryptionOptions.Encrypt, HelperService.cryptionAlgorithm, HelperService.importedPaths, compressionLevel, outputFileName, keyFileName, checkBoxIsSplitArchive.Checked ? (long)splitArchiveSize : 0, password);

                        frmProgress.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Please select both the input files and the output file for archiving.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            if (radioButtonDecompressDecrypt.Checked == true)
            {
                ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.DecompressDecryptSelectedItemsOnly, treeView1);
                frmProcessing.ShowDialog();

                string password = String.Empty;
                string keyFileName = String.Empty;
                string outputFileName = String.Empty;
                HelperService.OverwriteFilesSetting overwriteFilesSetting = HelperService.OverwriteFilesSetting.Ask;

                // If the file names are not an empty string, call corresponding methods for processing.

                var selectedPaths = new string[HelperService.selectedPaths.Count];
                var selectedPathsCorrespondingArchive = new string[selectedPaths.Length];

                try
                {
                    for (var i = 0; i < HelperService.selectedPaths.Count; i++)
                    {
                        dynamic pathObj = HelperService.selectedPaths[i];
                        selectedPaths[i] = pathObj.relativePath;
                        selectedPathsCorrespondingArchive[i] = pathObj.currentArchiveName;
                    }
                }
                catch { }

                if (HelperService.importedPaths.Count > 0 && textBoxOutputFile.Text.Trim() != String.Empty)
                {
                    if (textBoxKeyFile.Enabled == true && textBoxKeyFile.Text.Trim() == String.Empty)
                    {
                        MessageBox.Show("Please indicate a cryption key file or choose default built-in key.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (maskedTextBoxPassword.Enabled == true && string.IsNullOrEmpty(maskedTextBoxPassword.Text))
                    {
                        MessageBox.Show("Please make sure you have typed a password in order for the extraction to take place.",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (textBoxKeyFile.Enabled == true) keyFileName = textBoxKeyFile.Text.Trim();
                        if (maskedTextBoxPassword.Enabled == true) password = maskedTextBoxPassword.Text;

                        outputFileName = textBoxOutputFile.Text.Trim();

                        if (radioButtonOverwriteFilesAsk.Checked)
                        {
                            overwriteFilesSetting = HelperService.OverwriteFilesSetting.Ask;
                        }

                        if (radioButtonOverwriteFilesYes.Checked)
                        {
                            overwriteFilesSetting = HelperService.OverwriteFilesSetting.Yes;
                        }

                        if (radioButtonOverwriteFilesNo.Checked)
                        {
                            overwriteFilesSetting = HelperService.OverwriteFilesSetting.No;
                        }

                        ProgressForm frmProgress = new ProgressForm(HelperService.CryptionOptions.Decrypt, HelperService.cryptionAlgorithm, HelperService.importedPaths, selectedPaths, selectedPathsCorrespondingArchive, true, overwriteFilesSetting, HelperService.compressionLevel, outputFileName, keyFileName, 0, password);

                        frmProgress.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Please select both the input files and the output folder in order for the extraction to take place.",
                       "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /********************************************************************************************************************/

        private void btnCompressEncryptExtra_Click(object sender, EventArgs e)
        {
            menuCompressEncryptExtra = new ContextMenu(); // fix for menu item length stretch when switching between encrypt / decrypt
            menuCompressEncryptExtra.MenuItems.Add(menuCompressEncryptExtraItem1);
            if (!radioButtonCompressEncrypt.Checked)
            {
                menuCompressEncryptExtra.MenuItems.Add(menuCompressEncryptExtraItem3);
            }
            menuCompressEncryptExtra.Show(btnCompressEncryptExtra, new Point(-HelperService.FindLongestLengthOfElementText(menuCompressEncryptExtra.MenuItems) * 6 * HelperService.scaling / 100, btnCompressEncryptExtra.Size.Height));
        }

        /********************************************************************************************************************/

        private void btnUpdateArchive1_Click(object sender, EventArgs e)
        {
            menuUpdateArchive1.Show(btnUpdateArchive1, new Point(-HelperService.FindLongestLengthOfElementText(menuUpdateArchive1.MenuItems) * 6 * HelperService.scaling / 100, btnUpdateArchive1.Size.Height));
        }

        /********************************************************************************************************************/
        
        private void btnShortcut_Click(object sender, EventArgs e)
        {
            InputBoxForm inputBoxForm = new InputBoxForm("NHC Shortcut Generator", "Choose option:", HelperService.ConfirmationButtons.ShortcutGeneratorButtons);
            inputBoxForm.ShowDialog();

            if (inputBoxForm.Result == 1) // Create Shortcut 
            {
                saveFileDialog1.Filter = "Shortcut File|*.lnk|Other File|*.*";
                saveFileDialog1.Title = "Choose Output Shortcut File (with saved settings)";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    HelperService.CreateShortcut(saveFileDialog1.FileName);
                }
            }
            else
            if (inputBoxForm.Result == 2) // Create File Association
            {
                HelperService.CreateFileAssociation();
            }
            else
            if (inputBoxForm.Result == 3) // Create 'Send to...' Shortcut
            {
                HelperService.CreateSendToShortcut();
            }
        }

        /********************************************************************************************************************/

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Needle in a Haystack in a Crypt v1.0 (ENG)\r\n\r\nCopyright © 2016-2023 by Horia Nedelciuc\r\n\r\nThis is a file archiver that can compress and encrypt files and folders in NHC format. You can find the source code here:\r\n\r\nhttps://github.com/hnedelciuc/NHC\r\n\r\nLicensed under the Apache License, Version 2.0 (the \"License\"); you may not use this file except in compliance with the License. You may obtain a copy of the License at:\r\n\r\nhttp://www.apache.org/licenses/LICENSE-2.0\r\n\r\nUnless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.",
                "About NHC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /********************************************************************************************************************/

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm frmSettings = new SettingsForm();
            frmSettings.ShowDialog();
            SetSelectionSettings();
            SetButtonSettings();
            SetWindowColorSettings();
            AdjustDisplayScaling(true);
        }

        /********************************************************************************************************************/

        private void SetButtonSettings()
        {
            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsOnly) // Icons
            {
                btnCompressEncrypt.Image = radioButtonCompressEncrypt.Checked ? Properties.Resources.Encrypt__3D__small : Properties.Resources.Decrypt__3D__small;
                btnCompressEncrypt.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnCompressEncrypt.Text = "";
                toolTip1.SetToolTip(btnCompressEncrypt, radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive");
                btnBrowseInput.Image = Properties.Resources.Plus__3D__small;
                btnBrowseInput.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnBrowseInput.Text = "";
                toolTip1.SetToolTip(btnBrowseInput, "Browse");
                btnRemoveSelected.Image = Properties.Resources.Minus__3D__small;
                btnRemoveSelected.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnRemoveSelected.Text = "";
                toolTip1.SetToolTip(btnRemoveSelected, "Remove Selected");
                btnGenerateKey.Image = Properties.Resources.Key_Plus__3D__small;
                btnGenerateKey.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnGenerateKey.Text = "";
                toolTip1.SetToolTip(btnGenerateKey, "Generate Key");
                btnCheckKey.Image = Properties.Resources.Key_Check__3D__small;
                btnCheckKey.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnCheckKey.Text = "";
                toolTip1.SetToolTip(btnCheckKey, "Check Key");
                btnBrowseOutput.Image = Properties.Resources.Plus__3D__mini;
                btnBrowseOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnBrowseOutput.Text = "";
                toolTip1.SetToolTip(btnBrowseOutput, "Browse");
                btnBrowseKey.Image = Properties.Resources.Plus__3D__mini;
                btnBrowseKey.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnBrowseKey.Text = "";
                toolTip1.SetToolTip(btnBrowseKey, "Browse");
                btnShowHidePassword.Image = Properties.Resources.Eye__3D__mini;
                btnShowHidePassword.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnShowHidePassword.Text = "";
                toolTip1.SetToolTip(btnShowHidePassword, showPassword ? "Hide" : "Show");
                btnSettings.Image = Properties.Resources.Settings__3D__small;
                btnSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnSettings.Text = "";
                toolTip1.SetToolTip(btnSettings, "Settings");
                btnAbout.Image = Properties.Resources.Info__3D__small;
                btnAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnAbout.Text = "";
                toolTip1.SetToolTip(btnAbout, "About NHC");
                btnShortcut.Image = Properties.Resources.Shortcut__3D__small;
                btnShortcut.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnShortcut.Text = "";
                toolTip1.SetToolTip(btnShortcut, "Shortcut");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.TextOnly) // Text
            {
                btnCompressEncrypt.Image = null;
                btnCompressEncrypt.Text = radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive";
                btnCompressEncrypt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnCompressEncrypt, "");
                btnBrowseInput.Image = null;
                btnBrowseInput.Text = "Browse";
                btnBrowseInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnBrowseInput, "");
                btnRemoveSelected.Image = null;
                btnRemoveSelected.Text = "Remove Selected";
                btnRemoveSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnRemoveSelected, "");
                btnGenerateKey.Image = null;
                btnGenerateKey.Text = "Generate Key";
                btnGenerateKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnGenerateKey, "");
                btnCheckKey.Image = null;
                btnCheckKey.Text = "Check Key";
                btnCheckKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnCheckKey, "");
                btnBrowseOutput.Image = null;
                btnBrowseOutput.Text = "Browse";
                btnBrowseOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnBrowseOutput, "");
                btnBrowseKey.Image = null;
                btnBrowseKey.Text = "Browse";
                btnBrowseKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnBrowseKey, "");
                btnShowHidePassword.Image = null;
                btnShowHidePassword.Text = showPassword ? "Hide" : "Show";
                btnShowHidePassword.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnShowHidePassword, "");
                btnSettings.Image = null;
                btnSettings.Text = "Settings";
                btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnSettings, "");
                btnAbout.Image = null;
                btnAbout.Text = "About NHC";
                btnAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnAbout, "");
                btnShortcut.Image = null;
                btnShortcut.Text = "Shortcut";
                btnShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnShortcut, "");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsAndText) // Icons + Text
            {
                btnCompressEncrypt.Image = radioButtonCompressEncrypt.Checked ? Properties.Resources.Encrypt__3D__small_2 : Properties.Resources.Decrypt__3D__small_2;
                btnCompressEncrypt.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnCompressEncrypt.Text = radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive";
                btnCompressEncrypt.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnCompressEncrypt, "");
                btnBrowseInput.Image = Properties.Resources.Plus__3D__small_2;
                btnBrowseInput.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnBrowseInput.Text = "Browse";
                btnBrowseInput.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnBrowseInput, "");
                btnRemoveSelected.Image = Properties.Resources.Minus__3D__small_2;
                btnRemoveSelected.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnRemoveSelected.Text = "Remove Selected";
                btnRemoveSelected.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnRemoveSelected, "");
                btnGenerateKey.Image = Properties.Resources.Key_Plus__3D__small_2;
                btnGenerateKey.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnGenerateKey.Text = "Generate Key";
                btnGenerateKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnGenerateKey, "");
                btnCheckKey.Image = Properties.Resources.Key_Check__3D__small_2;
                btnCheckKey.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnCheckKey.Text = "Check Key";
                btnCheckKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnCheckKey, "");
                btnBrowseOutput.Image = Properties.Resources.Plus__3D__mini_2;
                btnBrowseOutput.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnBrowseOutput.Text = "Browse";
                btnBrowseOutput.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnBrowseOutput, "");
                btnBrowseKey.Image = Properties.Resources.Plus__3D__mini_2;
                btnBrowseKey.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnBrowseKey.Text = "Browse";
                btnBrowseKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnBrowseKey, "");
                btnShowHidePassword.Image = Properties.Resources.Eye__3D__mini_2;
                btnShowHidePassword.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnShowHidePassword.Text = showPassword ? "Hide" : "Show";
                btnShowHidePassword.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnShowHidePassword, "");
                btnSettings.Image = Properties.Resources.Settings__3D__small;
                btnSettings.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnSettings.Text = "Settings";
                btnSettings.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnSettings, "");
                btnAbout.Image = Properties.Resources.Info__3D__small;
                btnAbout.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnAbout.Text = "About NHC";
                btnAbout.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnAbout, "");
                btnShortcut.Image = Properties.Resources.Shortcut__3D__small;
                btnShortcut.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnShortcut.Text = "Shortcut";
                btnShortcut.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnShortcut, "");
            }
        }

        /********************************************************************************************************************/

        private void SetCompressEncryptButtonSetting()
        {
            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsOnly)
            {
                btnCompressEncrypt.Image = radioButtonCompressEncrypt.Checked ? Properties.Resources.Encrypt__3D__small : Properties.Resources.Decrypt__3D__small;
                btnCompressEncrypt.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                btnCompressEncrypt.Text = "";
                toolTip1.SetToolTip(btnCompressEncrypt, radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.TextOnly)
            {
                btnCompressEncrypt.Image = null;
                btnCompressEncrypt.Text = radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive";
                btnCompressEncrypt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                toolTip1.SetToolTip(btnCompressEncrypt, "");
            }

            if (HelperService.buttonSetting == HelperService.ButtonSetting.IconsAndText)
            {
                btnCompressEncrypt.Image = radioButtonCompressEncrypt.Checked ? Properties.Resources.Encrypt__3D__small_2 : Properties.Resources.Decrypt__3D__small_2;
                btnCompressEncrypt.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
                btnCompressEncrypt.Text = radioButtonCompressEncrypt.Checked ? "Create Archive" : "Extract Archive";
                btnCompressEncrypt.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                toolTip1.SetToolTip(btnCompressEncrypt, "");
            }

            btnCompressEncrypt.Image = HelperService.ResizeImagePercent(btnCompressEncrypt.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
        }

        /********************************************************************************************************************/

        private void SetSelectionSettings()
        {
            if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
            {
                treeView1.CheckBoxes = true;
            }

            if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
            {
                treeView1.CheckBoxes = false;
            }
        }

        /********************************************************************************************************************/
        
        private bool isManualCompressEncryptCheckChanged;
        private void SetCryptionSettings()
        {
            menuTreeViewItem3.Visible = false;
            menuTreeViewItem4.Visible = false;
            btnUpdateArchive1.Visible = false;

            isManualCompressEncryptCheckChanged = false;

            if (HelperService.cryptionSetting == HelperService.CryptionOptions.Encrypt)
            {
                radioButtonCompressEncrypt.Checked = true;
            }
            if (HelperService.cryptionSetting == HelperService.CryptionOptions.Decrypt)
            {
                radioButtonDecompressDecrypt.Checked = true;
            }

            isManualCompressEncryptCheckChanged = true;

            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptKey)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 0;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.NeedleCryptPassword)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 1;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.RC2Key)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 2;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.RC2Password)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 3;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.TripleDesKey)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 4;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.TripleDesPassword)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 5;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.AesKey)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 6;
            if (HelperService.cryptionAlgorithm == HelperService.CryptionAlgorithm.AesPassword)
                comboBoxChooseCryptionAlgorithm.SelectedIndex = 7;

            if (!string.IsNullOrEmpty(HelperService.keyFileName))
                radioButtonUseExistingKey.Checked = true;
            else radioButtonUseBuiltInDefault.Checked = true;

            textBoxKeyFile.Text = HelperService.keyFileName;
            maskedTextBoxPassword.Text = HelperService.pwd;
            textBoxOutputFile.Text = HelperService.output;
        }

        /********************************************************************************************************************/

        private void SetCompressionSettings()
        {
            if (HelperService.compressionLevel == CompressionLevel.Optimal)
                radioButtonCompressionHigh.Checked = true;
            else if (HelperService.compressionLevel == CompressionLevel.Fastest)
                radioButtonCompressionLow.Checked = true;
            else radioButtonCompressionNone.Checked = true;
        }

        /********************************************************************************************************************/

        private void SetWindowColorSettings()
        {
            switch (HelperService.windowColorSetting)
            {
                case HelperService.WindowColorSetting.LightSteelBlue:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
                case HelperService.WindowColorSetting.DarkGray:
                    this.BackColor = System.Drawing.Color.DarkGray;
                    this.panel1.BackColor = System.Drawing.Color.DarkGray;
                    break;
                case HelperService.WindowColorSetting.White:
                    this.BackColor = System.Drawing.Color.White;
                    this.panel1.BackColor = System.Drawing.Color.White;
                    break;
                default:
                    this.BackColor = System.Drawing.Color.LightSteelBlue;
                    this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
                    break;
            }
        }

        /********************************************************************************************************************/

        internal void removeAfterCheckEvent()
        {
            this.treeView1.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
        }

        /********************************************************************************************************************/

        internal void addAfterCheckEvent()
        {
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
        }

        /********************************************************************************************************************/

        internal void LoadInputFiles()
        {
            object[] paths;

            if (HelperService.argPaths == null || HelperService.argPaths.Count == 0)
                paths = HelperService.importedPaths.ToArray();
            else paths = HelperService.argPaths.ToArray();

            if (paths.Length == 0) return;

            if (HelperService.cryptionSetting == HelperService.CryptionOptions.Encrypt)
            {
                ProcessingForm frmProcessing = new ProcessingForm(HelperService.ProcessingTask.ImportFilesDirectories_LoadInputFiles, paths);
                frmProcessing.ShowDialog();

                treeView1.BeginUpdate();
                treeView1.SelectedNodes = frmProcessing.treeView1.SelectedNodes;
                ArrayList nodes = new ArrayList(frmProcessing.treeView1.Nodes);
                foreach (TreeNode node in nodes)
                {
                    node.Remove(); // remove from frmProcessing.treeView1
                    treeView1.Nodes.Add(node); // add to treeView1
                }
                treeView1.EndUpdate();
            }

            if (HelperService.cryptionSetting == HelperService.CryptionOptions.Decrypt)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    treeView1.BeginUpdate();

                    maskedTextBoxPassword.Text = string.IsNullOrEmpty(HelperService.pwd) ? maskedTextBoxPassword.Text : HelperService.pwd;
                    textBoxKeyFile.Text = string.IsNullOrEmpty(HelperService.keyFileName) ? textBoxKeyFile.Text : HelperService.keyFileName;

                    if (HelperService.compressionLevel == CompressionLevel.Optimal)
                        radioButtonCompressionHigh.Checked = true;
                    else if (HelperService.compressionLevel == CompressionLevel.Fastest)
                        radioButtonCompressionLow.Checked = true;
                    else
                        radioButtonCompressionNone.Checked = true;

                    for (var i = 0; i < paths.Length; i++)
                    {
                        dynamic p = paths[i];
                        switch ((HelperService.CryptionAlgorithm)p.cryptionAlgorithm)
                        {
                            case HelperService.CryptionAlgorithm.NeedleCryptKey:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 0;
                                break;
                            case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 1;
                                break;
                            case HelperService.CryptionAlgorithm.RC2Key:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 2;
                                break;
                            case HelperService.CryptionAlgorithm.RC2Password:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 3;
                                break;
                            case HelperService.CryptionAlgorithm.TripleDesKey:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 4;
                                break;
                            case HelperService.CryptionAlgorithm.TripleDesPassword:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 5;
                                break;
                            case HelperService.CryptionAlgorithm.AesKey:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 6;
                                break;
                            case HelperService.CryptionAlgorithm.AesPassword:
                                comboBoxChooseCryptionAlgorithm.SelectedIndex = 7;
                                break;
                        }

                        if (HelperService.rootNodes[i] != null)
                        {
                            treeView1.Nodes.Add(HelperService.rootNodes[i]);
                        }
                    }

                    HelperService.argPaths = null;

                    treeView1.TreeViewNodeSorter = new HelperService.NodeSorter();
                    treeView1.Sort();

                    treeView1.EndUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Cursor.Current = Cursors.Default;
            }
        }

        /********************************************************************************************************************/

        private void InitializeOtherStuff()
        {
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.NeedleCryptKey], 0));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.NeedleCryptPassword], 1));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.RC2Key], 2));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.RC2Password], 3));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.TripleDesKey], 4));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.TripleDesPassword], 5));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.AesKey], 6));
            comboBoxChooseCryptionAlgorithm.Items.Add(new Item(HelperService.cryptionAlgorithmDict[HelperService.CryptionAlgorithm.AesPassword], 7));

            menuTreeView.MenuItems.Add(menuTreeViewItem1);
            menuTreeView.MenuItems.Add(menuTreeViewItem3);
            menuTreeView.MenuItems.Add(menuTreeViewItem4);
            menuTreeView.MenuItems.Add(menuTreeViewItem5);
            menuTreeViewItem1.Click += new EventHandler(menuTreeViewItem1_Click);
            menuTreeViewItem3.Click += new EventHandler(menuTreeViewItem3_Click);
            menuTreeViewItem4.Click += new EventHandler(menuTreeViewItem4_Click);
            menuTreeViewItem5.Click += new EventHandler(menuTreeViewItem5_Click);

            menuCompressEncryptExtra.MenuItems.Add(menuCompressEncryptExtraItem1);
            menuCompressEncryptExtra.MenuItems.Add(menuCompressEncryptExtraItem3);
            menuCompressEncryptExtraItem1.Click += new EventHandler(menuCompressEncryptExtraItem1_Click);
            menuCompressEncryptExtraItem3.Click += new EventHandler(menuCompressEncryptExtraItem3_Click);

            menuUpdateArchive1.MenuItems.Add(menuUpdateArchive1Item1);
            menuUpdateArchive1.MenuItems.Add(menuUpdateArchive1Item3);
            menuUpdateArchive1Item1.Click += new EventHandler(menuUpdateArchive1Item2_Click);
            menuUpdateArchive1Item3.Click += new EventHandler(menuUpdateArchive1Item3_Click);

            HelperService.exitOnClose = true;
        }

        /********************************************************************************************************************/

        // Content item for the combo box
        private class Item
        {
            internal string Name;
            internal int Value;
            internal Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                // Generates the text shown in the combo box
                return Name;
            }
        }

        /********************************************************************************************************************/

        private void comboBoxChooseCryptionAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item item = (Item)comboBoxChooseCryptionAlgorithm.SelectedItem;
            HelperService.cryptionAlgorithm = (HelperService.CryptionAlgorithm)item.Value;

            switch (HelperService.cryptionAlgorithm)
            {
                case HelperService.CryptionAlgorithm.NeedleCryptKey:
                    label5.Enabled = false;
                    maskedTextBoxPassword.Enabled = false;
                    btnShowHidePassword.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = true;

                    if (radioButtonUseBuiltInDefault.Checked == true)
                    {
                        label3.Enabled = false;
                        textBoxKeyFile.Enabled = false;
                        btnBrowseKey.Enabled = false;
                        btnGenerateKey.Enabled = false;
                        btnCheckKey.Enabled = false;
                    }
                    if (radioButtonUseExistingKey.Checked == true)
                    {
                        btnGenerateKey.Enabled = false;
                        btnCheckKey.Enabled = true;
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                    }
                    if (radioButtonGenerateNewKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnGenerateKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                    }
                    break;
                case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                    label5.Enabled = true;
                    maskedTextBoxPassword.Enabled = true;
                    btnShowHidePassword.Enabled = true;
                    groupBoxChooseCryptionKey.Enabled = true;

                    if (radioButtonUseBuiltInDefault.Checked == true)
                    {
                        label3.Enabled = false;
                        textBoxKeyFile.Enabled = false;
                        btnBrowseKey.Enabled = false;
                        btnGenerateKey.Enabled = false;
                        btnCheckKey.Enabled = false;
                    }
                    if (radioButtonUseExistingKey.Checked == true)
                    {
                        btnGenerateKey.Enabled = false;
                        btnCheckKey.Enabled = true;
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                    }
                    if (radioButtonGenerateNewKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnGenerateKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                    }
                    break;
                case HelperService.CryptionAlgorithm.RC2Key:
                    label5.Enabled = false;
                    maskedTextBoxPassword.Enabled = false;
                    btnShowHidePassword.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = true;

                    if (radioButtonUseBuiltInDefault.Checked == true)
                    {
                        label3.Enabled = false;
                        textBoxKeyFile.Enabled = false;
                        btnBrowseKey.Enabled = false;
                        btnCheckKey.Enabled = false;
                    }
                    if (radioButtonUseExistingKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                    }
                    if (radioButtonGenerateNewKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                        btnGenerateKey.Enabled = true;
                    }
                    break;
                case HelperService.CryptionAlgorithm.RC2Password:
                    label3.Enabled = false;
                    textBoxKeyFile.Enabled = false;
                    btnBrowseKey.Enabled = false;
                    btnCheckKey.Enabled = false;
                    btnGenerateKey.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = false;
                    label5.Enabled = true;
                    maskedTextBoxPassword.Enabled = true;
                    btnShowHidePassword.Enabled = true;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesKey:
                    label5.Enabled = false;
                    maskedTextBoxPassword.Enabled = false;
                    btnShowHidePassword.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = true;

                    if (radioButtonUseBuiltInDefault.Checked == true)
                    {
                        label3.Enabled = false;
                        textBoxKeyFile.Enabled = false;
                        btnBrowseKey.Enabled = false;
                        btnCheckKey.Enabled = false;
                    }
                    if (radioButtonUseExistingKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                    }
                    if (radioButtonGenerateNewKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                        btnGenerateKey.Enabled = true;
                    }
                    break;
                case HelperService.CryptionAlgorithm.TripleDesPassword:
                    label3.Enabled = false;
                    textBoxKeyFile.Enabled = false;
                    btnBrowseKey.Enabled = false;
                    btnCheckKey.Enabled = false;
                    btnGenerateKey.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = false;
                    label5.Enabled = true;
                    maskedTextBoxPassword.Enabled = true;
                    btnShowHidePassword.Enabled = true;
                    break;
                case HelperService.CryptionAlgorithm.AesKey:
                    label5.Enabled = false;
                    maskedTextBoxPassword.Enabled = false;
                    btnShowHidePassword.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = true;

                    if (radioButtonUseBuiltInDefault.Checked == true)
                    {
                        label3.Enabled = false;
                        textBoxKeyFile.Enabled = false;
                        btnBrowseKey.Enabled = false;
                        btnCheckKey.Enabled = false;
                    }
                    if (radioButtonUseExistingKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                    }
                    if (radioButtonGenerateNewKey.Checked == true)
                    {
                        label3.Enabled = true;
                        textBoxKeyFile.Enabled = true;
                        btnBrowseKey.Enabled = true;
                        btnCheckKey.Enabled = true;
                        btnGenerateKey.Enabled = true;
                    }
                    break;
                case HelperService.CryptionAlgorithm.AesPassword:
                    label3.Enabled = false;
                    textBoxKeyFile.Enabled = false;
                    btnBrowseKey.Enabled = false;
                    btnCheckKey.Enabled = false;
                    btnGenerateKey.Enabled = false;
                    groupBoxChooseCryptionKey.Enabled = false;
                    label5.Enabled = true;
                    maskedTextBoxPassword.Enabled = true;
                    btnShowHidePassword.Enabled = true;
                    break;
                default: break;
            }
        }

        /********************************************************************************************************************/

        private void comboBoxSplitArchiveSize_TextUpdate(object sender, EventArgs e)
        {
            var value = comboBoxSplitArchiveSize.Text ?? string.Empty;

            var regex1 = new Regex("((\\d+)\\.(\\d*))|(\\d+)");
            var regex2 = new Regex("((\\d+)\\,(\\d*))|(\\d+)");
            
            var match1 = regex1.Match(value);
            var match2 = regex2.Match(value);
            var lastChar = (value.Length >= 1)
                ? value.ToUpper()[value.Length - 1]
                : 'X';

            var match = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".")
                ? match1
                : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," ? match2 : null;

            if (match != null && match.Success)
            {
                var newValue = (
                    ((lastChar == 'K') || (lastChar == 'M') || (lastChar == 'G') || (lastChar == 'T') || (lastChar == ' ')) && prevComboBoxSplitArchiveSizeText.EndsWith(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        ? match.Value.Substring(0, match.Value.Length - 1) : match.Value) +
                    ((lastChar == 'K' && value.Length > prevComboBoxSplitArchiveSizeText.Length) ? (prevComboBoxSplitArchiveSizeText.EndsWith("KB") ? " " : " KB") :
                    (lastChar == 'M' && value.Length > prevComboBoxSplitArchiveSizeText.Length) ? (prevComboBoxSplitArchiveSizeText.EndsWith("MB") ? " " : " MB") :
                    (lastChar == 'G' && value.Length > prevComboBoxSplitArchiveSizeText.Length) ? (prevComboBoxSplitArchiveSizeText.EndsWith("GB") ? " " : " GB") :
                    (lastChar == 'T' && value.Length > prevComboBoxSplitArchiveSizeText.Length) ? (prevComboBoxSplitArchiveSizeText.EndsWith("TB") ? " " : " TB") :
                    (lastChar == ' ') ? " " : string.Empty);

                if (comboBoxSplitArchiveSize.Items.Count > 8) // 8 is the nr of items predefined in the dropdown list
                {
                    comboBoxSplitArchiveSize.Items.RemoveAt(comboBoxSplitArchiveSize.Items.Count - 1);
                }

                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    if (!comboBoxSplitArchiveSize.Items.Contains(newValue))
                        comboBoxSplitArchiveSize.SelectedIndex = comboBoxSplitArchiveSize.Items.Add(newValue);
                    else comboBoxSplitArchiveSize.SelectedIndex = comboBoxSplitArchiveSize.Items.IndexOf(newValue);
                }
            }
            else if (comboBoxSplitArchiveSize.Items.Count > 8) // 8 is the nr of items predefined in the dropdown list
            {
                comboBoxSplitArchiveSize.Items.RemoveAt(comboBoxSplitArchiveSize.Items.Count - 1);
            }
            else comboBoxSplitArchiveSize.Text = string.Empty;

            comboBoxSplitArchiveSize.SelectionStart = comboBoxSplitArchiveSize.Text?.Length ?? 0;

            prevComboBoxSplitArchiveSizeText = comboBoxSplitArchiveSize.SelectedItem?.ToString() ?? string.Empty;
        }

        /********************************************************************************************************************/

        private void comboBoxSplitArchiveSize_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxSplitArchiveSize.SelectedIndex = comboBoxSplitArchiveSize.SelectedIndex == comboBoxSplitArchiveSize.Items.Count
                ? -1
                : string.IsNullOrEmpty(comboBoxSplitArchiveSize.Text)
                    ? -1
                    : comboBoxSplitArchiveSize.Items.IndexOf(comboBoxSplitArchiveSize.Text);

            comboBoxSplitArchiveSize.SelectionStart = comboBoxSplitArchiveSize.Text?.Length ?? 0;
        }

        /********************************************************************************************************************/

        private void comboBoxSplitArchiveSize_DropDownClosed(object sender, EventArgs e)
        {
            comboBoxSplitArchiveSize.SelectedIndex = comboBoxSplitArchiveSize.SelectedIndex == comboBoxSplitArchiveSize.Items.Count 
                ? -1 : comboBoxSplitArchiveSize.SelectedIndex;

            comboBoxSplitArchiveSize.SelectionStart = comboBoxSplitArchiveSize.Text?.Length ?? 0;
        }

        /********************************************************************************************************************/

        private double? ValidateSplitArchiveSize()
        {
            double splitArchiveSize = 0;

            if (checkBoxIsSplitArchive.Checked)
            {
                var splitArchiveSizeStr = comboBoxSplitArchiveSize.Text.ToUpper();
                var splitArchiveSizeArr = splitArchiveSizeStr.Split(" "[0]);

                var suffixCount = splitArchiveSizeArr.Count(el => el == "TB") + splitArchiveSizeArr.Count(el => el == "GB") + splitArchiveSizeArr.Count(el => el == "MB") + splitArchiveSizeArr.Count(el => el == "KB");

                if (suffixCount == 1)
                {
                    if (splitArchiveSizeArr.Contains("TB"))
                    {
                        double.TryParse(splitArchiveSizeStr.Trim().Replace("TB", "").Trim(), out splitArchiveSize);

                        if (splitArchiveSize > 0)
                        {
                            splitArchiveSize = (long)(splitArchiveSize * 1024 * 1024 * 1024 * 1024);
                        }
                    }
                    else if (splitArchiveSizeStr.Contains("GB"))
                    {
                        double.TryParse(splitArchiveSizeStr.Trim().Replace("GB", "").Trim(), out splitArchiveSize);

                        if (splitArchiveSize > 0)
                        {
                            splitArchiveSize = (long)(splitArchiveSize * 1024 * 1024 * 1024);
                        }
                    }
                    else if (splitArchiveSizeStr.Contains("MB"))
                    {
                        double.TryParse(splitArchiveSizeStr.Trim().Replace("MB", "").Trim(), out splitArchiveSize);

                        if (splitArchiveSize > 0)
                        {
                            splitArchiveSize = (long)(splitArchiveSize * 1024 * 1024);
                        }
                    }
                    else if (splitArchiveSizeStr.Contains("KB"))
                    {
                        double.TryParse(splitArchiveSizeStr.Trim().Replace("KB", "").Trim(), out splitArchiveSize);

                        if (splitArchiveSize > 0)
                        {
                            splitArchiveSize = (long)(splitArchiveSize * 1024);
                        }
                    }
                }
                else if (suffixCount == 0)
                {
                    double.TryParse(splitArchiveSizeStr.Trim(), out splitArchiveSize);
                    splitArchiveSize = (long)splitArchiveSize;
                }

                if (splitArchiveSize < 131072 /* 128 KB */)
                {
                    MessageBox.Show("Please enter a valid maximum split archive size. You may use KB, MB, GB or TB to indicate max size. Decimals are allowed. If a raw number (without KB or MB, etc) is used, then the value will be understood to be referring to \"bytes\". The minimum value must be the equivalent of 128 x 1024 bytes or greater.", "Invalid Maximum Split Archive Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comboBoxSplitArchiveSize.Text = "";

                    return null;
                }
            }
            else return null;

            return splitArchiveSize;
        }

        /********************************************************************************************************************/

        internal void GetDisplayScaleFactor()
        {
            float dpiX;
            Graphics graphics = CreateGraphics();
            dpiX = graphics.DpiX;
            HelperService.scaling = (int)(dpiX * 100 / 96);
        }

        /********************************************************************************************************************/
        
        private void AdjustDisplayScaling(bool preventResizeTreeView)
        {
            btnBrowseInput.Image = HelperService.ResizeImagePercent(btnBrowseInput.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
            btnCheckKey.Image = HelperService.ResizeImagePercent(btnCheckKey.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
            btnCompressEncrypt.Image = HelperService.ResizeImagePercent(btnCompressEncrypt.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
            btnGenerateKey.Image = HelperService.ResizeImagePercent(btnGenerateKey.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
            btnRemoveSelected.Image = HelperService.ResizeImagePercent(btnRemoveSelected.Image, HelperService.buttonSetting == 0 ? (HelperService.scaling * 7 / 10) : (HelperService.scaling * 9 / 10));
            btnBrowseKey.Image = HelperService.ResizeImagePercent(btnBrowseKey.Image, HelperService.scaling);
            btnBrowseOutput.Image = HelperService.ResizeImagePercent(btnBrowseOutput.Image, HelperService.scaling); 
            btnSettings.Image = HelperService.ResizeImagePercent(btnSettings.Image, HelperService.buttonSetting == 0 ? HelperService.scaling * 7 / 10 : HelperService.scaling * 5 / 10);
            btnAbout.Image = HelperService.ResizeImagePercent(btnAbout.Image, HelperService.buttonSetting == 0 ? HelperService.scaling * 7 / 10 : HelperService.scaling * 5 / 10);
            btnShortcut.Image = HelperService.ResizeImagePercent(btnShortcut.Image, HelperService.buttonSetting == 0 ? HelperService.scaling * 7 / 10  : HelperService.scaling * 5 / 10);
            btnShowHidePassword.Image = HelperService.ResizeImagePercent(btnShowHidePassword.Image, HelperService.scaling);

            if (!preventResizeTreeView) HelperService.InitializeImageList(imageList1);
        }
    }
}

/********************************************************************************************************************/
