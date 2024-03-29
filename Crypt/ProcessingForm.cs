﻿//********************************************************************************************************************//
// Needle in a Haystack in a Crypt v1.0.
// Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
//********************************************************************************************************************//
// Processing bar window.
//********************************************************************************************************************//

using GracefulDynamicDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Crypt
{
    internal partial class ProcessingForm : Form
    {
        /********************************************************************************************************************/

        internal ProcessingForm(HelperService.ProcessingTask taskId, ArrayList paths)
        {
            InitializeComponent();
            SetWindowColor();
            this.taskId = taskId;
            if (taskId == HelperService.ProcessingTask.OpenArchive)
                this.paths = paths;
            else addedNodes = paths;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
        }

        /********************************************************************************************************************/

        internal ProcessingForm(HelperService.ProcessingTask taskId, string[] paths)
        {
            InitializeComponent();
            SetWindowColor();
            this.taskId = taskId;
            this.paths = new ArrayList(paths);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
        }

        /********************************************************************************************************************/

        internal ProcessingForm(HelperService.ProcessingTask taskId, object[] pathObjs)
        {
            InitializeComponent();
            SetWindowColor();
            this.taskId = taskId;
            this.paths = new ArrayList(pathObjs);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
        }

        /********************************************************************************************************************/

        internal ProcessingForm(HelperService.ProcessingTask taskId, TreeNode clickedNode, TreeViewMS treeView1)
        {
            InitializeComponent();
            SetWindowColor();
            this.taskId = taskId;
            this.clickedNode = clickedNode;
            this.treeView1 = treeView1;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
        }

        /********************************************************************************************************************/

        internal ProcessingForm(HelperService.ProcessingTask taskId, TreeViewMS treeView1)
        {
            InitializeComponent();
            SetWindowColor();
            this.taskId = taskId;
            this.treeView1 = treeView1;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 10;
        }

        /********************************************************************************************************************/

        // Incoming parameter variables
        private HelperService.ProcessingTask taskId;
        private ArrayList paths;
        private ArrayList addedNodes;
        internal TreeViewMS treeView1 = new TreeViewMS();
        private TreeNode clickedNode;
        internal string passPhrase = string.Empty;
        internal string keyFileName = string.Empty;
        private byte[] key;
        private byte[] keyArray;
        private byte[] IV;

        /********************************************************************************************************************/

        // Local variables
        private bool workerIsDone = false;
        private string inputFileName = String.Empty;
        private string resultMessage = String.Empty;
        private int decryptCount = 0;
        private string currentFileProcessed = String.Empty;
        DateTime startDateTime;
        DateTime currentDateTime;
        private long selectedNodesCount = 0;
        private long uncompressedFilesSize = 0;
        private long uncompressedFileSize = 0;
        private long compressedFilesSize = 0;
        private long compressedHeadersSize = 0;
        private long selectedArchives = 0;
        private string relativePath = null;
        private string fullPath = null;

        /********************************************************************************************************************/

        // Output variables
        internal bool? extractAll { get; set; }

        /********************************************************************************************************************/

        private void Progress_Load(object sender, System.EventArgs e)
        {
            if (!worker.IsBusy && !workerIsDone)
            {
                worker.RunWorkerAsync();
            }
            switch (taskId)
            {
                case HelperService.ProcessingTask.OpenArchive: labelOutput.Text = "Opening Archive..."; break;
                case HelperService.ProcessingTask.ImportFilesDirectories_btnBrowseInput_Click:
                case HelperService.ProcessingTask.ImportFilesDirectories_treeView1_DragDrop:
                case HelperService.ProcessingTask.ImportFilesDirectories_LoadInputFiles: labelOutput.Text = "Importing Files && Directories..."; break;
                case HelperService.ProcessingTask.InfoRegardingSelectedEntries: labelOutput.Text = "Retrieving Info Regarding Selected Entries..."; break;
                case HelperService.ProcessingTask.DecompressDecryptSelectedItemsOnly: labelOutput.Text = "Preparing for extraction..."; break;
                case HelperService.ProcessingTask.UpdateArchiveContents: labelOutput.Text = "Updating Archive Contents..."; break;
            }
        }

        /********************************************************************************************************************/

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (taskId)
            {
                case HelperService.ProcessingTask.OpenArchive:
                    {
                        long nrOfFilesProcessed = 0;

                        worker.ReportProgress(0);

                        startDateTime = DateTime.Now;
                        nrOfFilesProcessed = 0;

                        HelperService.rootNodes = new TreeNode[paths.Count];
                        HelperService.compressedFilesSize = 0;
                        HelperService.compressedHeadersSize = 0;
                        HelperService.archiveFileSize = 0;
                        HelperService.uncompressedFilesSize = 0;

                        try
                        {
                            for (int i = 0; i < paths.Count; i++)
                            {
                                dynamic item = paths[i];
                                inputFileName = item.fullPath;

                                CryptionService.LoadKey(null, out keyArray, out IV, HelperService.CryptionAlgorithm.NeedleCryptKey, item.password);
                                CryptionService.LoadKey(item.keyFilePath, out key, out IV, item.cryptionAlgorithm, item.password);

                                HelperService.rootNodes[i] = OpenArchiveService.OpenArchive(i, paths.Count, inputFileName, (HelperService.CryptionAlgorithm)item.cryptionAlgorithm, (string)item.password, key, keyArray, IV,
                                    (current) => worker.ReportProgress(current), (current) => currentFileProcessed = current, (nr) => nrOfFilesProcessed += nr, (current) => currentDateTime = current);
                                decryptCount++;
                            }

                        }
                        catch (Exception exc)
                        {
                            resultMessage = exc.Message;
                        }

                        worker.ReportProgress(100);

                        workerIsDone = true;

                        if (decryptCount != paths.Count)
                        {
                            var s = paths.Count > 1 ? "s" : "";

                            if (decryptCount > 0)
                            {
                                // Displays the Success Message.
                                resultMessage = "Partial Success / Partial Fail" + Environment.NewLine + Environment.NewLine +
                                    decryptCount + " archived files have been opened successfully, while " +
                                    (paths.Count - decryptCount) + " have failed, and the failure occured at " +
                                    inputFileName + " file." + Environment.NewLine + Environment.NewLine +
                                    resultMessage + Environment.NewLine + Environment.NewLine +
                                    nrOfFilesProcessed + $" files from inside the archive{s} have been processed.";
                            }
                            else
                            {
                                resultMessage = "Fail" + Environment.NewLine + Environment.NewLine +
                                    $"Archived file{s} could not be opened." +
                                Environment.NewLine + Environment.NewLine + resultMessage +
                                Environment.NewLine + Environment.NewLine + nrOfFilesProcessed + $" files from inside the archive{s} have been processed.";

                            }

                            HelperService.importedPaths.RemoveAt(HelperService.importedPaths.Count - 1);
                        }

                        break;
                    }

                case HelperService.ProcessingTask.ImportFilesDirectories_btnBrowseInput_Click:
                    {
                        var errors = 0;

                        try
                        {
                            HelperService.backgroundWorkerClosePending = false;

                            treeView1.BeginUpdate();

                            // List all added entries in the main treeView1 and update paths.
                            foreach (TreeNode node in addedNodes)
                            {
                                if (LongDirectory.Exists(node.Name))
                                    HelperService.ListDirectory(treeView1, node.Name);
                                else
                                {
                                    if (!treeView1.Nodes.Contains(node))
                                    {
                                        var stream = LongFile.GetFileStream(node.Name);
                                        var uncompressedFileSize = stream.Length;
                                        HelperService.uncompressedFilesSize += uncompressedFileSize;
                                        stream.Close();
                                        stream.Dispose();
                                        treeView1.Nodes.Add(new TreeNode(node.Text)
                                        {
                                            Name = node.FullPath,
                                            ImageIndex = 1,
                                            SelectedImageIndex = 1,
                                            Tag = new DDict(new Dictionary<string, dynamic>()
                                            {
                                                { "relativePath", node.Text },
                                                { "fullPath", node.Name },
                                                { "isArchiveRoot", false }
                                            })
                                        });
                                        HelperService.importedPaths.Add(new DDict(new Dictionary<string, dynamic>()
                                        {
                                            { "relativePath", node.Text },
                                            { "fullPath", node.Name },
                                            { "isDirectory", false},
                                            { "uncompressedFileSize", uncompressedFileSize }
                                        }));
                                    }
                                }
                            }

                            if (!HelperService.backgroundWorkerClosePending)
                            {
                                treeView1.TreeViewNodeSorter = new HelperService.NodeSorter();
                                treeView1.Sort();
                            }

                            treeView1.EndUpdate();
                        }
                        catch { errors++; }

                        if (errors > 0)
                        {
                            MessageBox.Show($"There were {errors} errors when importing. Some files or folders may have not been imported.");
                        }

                        break;
                    }

                case HelperService.ProcessingTask.ImportFilesDirectories_treeView1_DragDrop:
                    {
                        var errors = 0;

                        try
                        {
                            HelperService.backgroundWorkerClosePending = false;

                            treeView1.BeginUpdate();

                            foreach (string path in paths)
                            {
                                string filename = Path.GetFileName(path);
                                if (LongDirectory.Exists(path))
                                {
                                    HelperService.ListDirectory(treeView1, path);
                                }
                                else
                                {
                                    long uncompressedFileSize = 0;
                                    if (LongFile.Exists(path))
                                    {
                                        using (var handle = LongFile.GetFileHandleWithRead(path))
                                        using (var stream = new FileStream(handle, FileAccess.Read))
                                        {
                                            uncompressedFileSize = stream.Length;
                                        }

                                        HelperService.uncompressedFilesSize += uncompressedFileSize;
                                    }
                                    var pathObj = new DDict(new Dictionary<string, dynamic>()
                                    {
                                        { "relativePath", filename },
                                        { "fullPath", path },
                                        { "isDirectory", false },
                                        { "uncompressedFileSize", uncompressedFileSize }
                                    });
                                    if (!HelperService.importedPaths.Contains(pathObj))
                                    {
                                        treeView1.Nodes.Add(new TreeNode(filename)
                                        {
                                            ImageIndex = 1,
                                            SelectedImageIndex = 1,
                                            Tag = pathObj
                                        });
                                        HelperService.importedPaths.Add(pathObj);
                                    }
                                }
                            }

                            if (!HelperService.backgroundWorkerClosePending)
                            {
                                treeView1.TreeViewNodeSorter = new HelperService.NodeSorter();
                                treeView1.Sort();
                            }

                            treeView1.EndUpdate();
                        }
                        catch { errors++; }

                        if (errors > 0)
                        {
                            MessageBox.Show($"There were {errors} errors when importing. Some files or folders may have not been imported.");
                        }

                        break;
                    }

                case HelperService.ProcessingTask.ImportFilesDirectories_LoadInputFiles:
                    {
                        var errors = 0;

                        try
                        {
                            HelperService.backgroundWorkerClosePending = false;

                            treeView1.BeginUpdate();

                            foreach (dynamic path in paths)
                            {
                                if (LongDirectory.Exists(path.fullPath))
                                    HelperService.ListDirectory(treeView1, path.fullPath);
                                else
                                {
                                    long uncompressedFileSize = 0;

                                    using (var handle = LongFile.GetFileHandleWithRead(path))
                                    using (var stream = new FileStream(handle, FileAccess.Read))
                                    {
                                        uncompressedFileSize = stream.Length;
                                    }

                                    HelperService.uncompressedFilesSize += uncompressedFileSize;

                                    var pathObj = new DDict(new Dictionary<string, dynamic>()
                                    {
                                        { "relativePath", path.relativePath },
                                        { "fullPath", path.fullPath },
                                        { "isDirectory", false },
                                        { "uncompressedFileSize", uncompressedFileSize }
                                    });

                                    if (!HelperService.importedPaths.Contains(pathObj))
                                    {
                                        treeView1.Nodes.Add(new TreeNode(path.relativePath)
                                        {
                                            ImageIndex = 1,
                                            SelectedImageIndex = 1,
                                            Tag = pathObj
                                        });
                                        HelperService.importedPaths.Add(pathObj);
                                    }
                                }
                            }
                            if (!HelperService.backgroundWorkerClosePending)
                            {
                                treeView1.TreeViewNodeSorter = new HelperService.NodeSorter();
                                treeView1.Sort();
                            }

                            treeView1.EndUpdate();
                        }
                        catch { errors++; }

                        if (errors > 0)
                        {
                            MessageBox.Show($"There were {errors} errors when importing. Some files or folders may have not been imported.");
                        }

                        break;
                    }

                case HelperService.ProcessingTask.InfoRegardingSelectedEntries:
                    {
                        ArrayList selectedNodes = new ArrayList();

                        if (treeView1.CheckBoxes)
                        {
                            HelperService.GetCheckedNodes(treeView1.Nodes, ref selectedNodes, true);
                            clickedNode = selectedNodes.Count > 0 ? (TreeNode)selectedNodes[0] : clickedNode;
                        }
                        else HelperService.GetSelectedNodes(treeView1.SelectedNodes, ref selectedNodes);

                        selectedNodesCount = selectedNodes.Count;

                        if (selectedNodesCount == 0)
                            return;

                        uncompressedFilesSize = 0;
                        compressedFilesSize = 0;
                        compressedHeadersSize = 0;
                        selectedArchives = 0;

                        if (HelperService.cryptionSetting == HelperService.CryptionOptions.Encrypt)
                        {
                            foreach (dynamic path in HelperService.importedPaths)
                            {
                                if (HelperService.backgroundWorkerClosePending)
                                    break;

                                if (relativePath == null && path.relativePath == ((dynamic)clickedNode.Tag).relativePath)
                                {
                                    relativePath = path.relativePath;
                                    fullPath = path.fullPath;
                                    uncompressedFileSize = path.uncompressedFileSize;
                                }

                                foreach (TreeNode node in selectedNodes)
                                {
                                    if (HelperService.backgroundWorkerClosePending)
                                        break;

                                    if (path.relativePath == ((dynamic)node.Tag)?.relativePath)
                                    {
                                        uncompressedFilesSize += path.uncompressedFileSize;
                                    }
                                }
                            }
                        }
                        else // decrypt
                        {
                            foreach (dynamic node in selectedNodes)
                            {
                                if (HelperService.backgroundWorkerClosePending)
                                    break;

                                // node.Tag is DDict, it won't throw exception if value not found

                                uncompressedFilesSize += node.Tag?.uncompressedFileSize ?? 0;
                                compressedFilesSize += (long)(((dynamic)node.Tag)?.compressedFullFileSize ?? ((dynamic)node.Tag)?.compressedFileSize ?? 0);
                                compressedHeadersSize += (long)(((dynamic)node.Tag)?.compressedFullHeaderSize ?? ((dynamic)node.Tag)?.compressedHeaderSize ?? 0);
                                
                                bool isArchiveRoot = isArchiveRoot = node.Tag?.isArchiveRoot ?? false;
                                
                                if (isArchiveRoot) selectedArchives++;
                            }
                        }

                        HelperService.backgroundWorkerClosePending = false;

                        break;
                    }

                case HelperService.ProcessingTask.DecompressDecryptSelectedItemsOnly:
                    {
                        ArrayList selectedNodes = new ArrayList();
                        HelperService.selectedPaths.Clear();
                        HelperService.numberOfEntriesProcessed = 0;

                        if (treeView1.CheckBoxes)
                            HelperService.GetCheckedNodes(treeView1.Nodes, ref selectedNodes, true);
                        else
                            HelperService.GetSelectedNodes(treeView1.SelectedNodes, ref selectedNodes);

                        if (selectedNodes.Count == 0)
                            HelperService.SelectAllNodes(treeView1.Nodes, true, ref selectedNodes);
                        
                        foreach (TreeNode node in selectedNodes)
                        {
                            if (HelperService.backgroundWorkerClosePending)
                                break;

                            var dynamicNode = (dynamic)node;
                            var tmpNode = (dynamic)node;
                            var isArchiveRoot = dynamicNode.Tag == null;
                            var currentArchiveName = "";

                            // node.Tag is DDict, it won't throw exception if value not found

                            while (true)
                            {
                                if (tmpNode == null) break;

                                if (tmpNode.Tag.isArchiveRoot == true)
                                {
                                    currentArchiveName = tmpNode.Name;
                                    break;
                                }
                                tmpNode = tmpNode.Parent;
                            }

                            if (currentArchiveName != "" && !isArchiveRoot && dynamicNode.Tag.relativePath != null && dynamicNode.Tag.isDirectory != null)
                                HelperService.selectedPaths.Add(new DDict(new Dictionary<string, dynamic>()
                                {
                                    { "relativePath", dynamicNode.Tag.relativePath },
                                    { "fullPath", "" },
                                    { "isDirectory", dynamicNode.Tag.isDirectory },
                                    { "currentArchiveName", currentArchiveName }
                                }));
                        }

                        HelperService.selectedPaths = new ArrayList(HelperService.selectedPaths.ToArray().Distinct().ToArray());

                        if (HelperService.backgroundWorkerClosePending)
                            HelperService.selectedPaths = new ArrayList();

                        break;
                    }

                case HelperService.ProcessingTask.UpdateArchiveContents:
                    {
                        var selectedNodes = new ArrayList();
                        var allNodes = new ArrayList();

                        HelperService.selectedPaths.Clear();
                        HelperService.entriesProcessed.Clear();
                        HelperService.numberOfEntriesProcessed = 0;

                        if (HelperService.selectionSetting == HelperService.SelectionSetting.Checkboxes)
                        {
                            HelperService.GetCheckedNodes(treeView1.Nodes, ref selectedNodes, true);
                        }
                        else if (HelperService.selectionSetting == HelperService.SelectionSetting.Multiselect)
                        {
                            selectedNodes = treeView1.SelectedNodes;
                        }

                        HelperService.SelectAllNodes(treeView1.Nodes, true, ref allNodes);

                        if (selectedNodes.Count == 0)
                        {
                            selectedNodes = allNodes;
                            extractAll = null;
                        }
                        else if (selectedNodes.Count == allNodes.Count) extractAll = true;
                        else extractAll = false;

                        try
                        {
                            string tempDirectory = LongDirectory.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            while (LongDirectory.Exists(tempDirectory))
                            {
                                tempDirectory = LongDirectory.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            }

                            var currentArchiveName = "";

                            foreach (TreeNode node in selectedNodes)
                            {
                                if (HelperService.backgroundWorkerClosePending)
                                    break;

                                // node.Tag is DDict, it won't throw exception if value not found

                                var dynamicNode = (dynamic)node;
                                var tmpNode = (dynamic)node;
                                var isArchiveRoot = dynamicNode.Tag.isArchiveRoot ?? false;

                                if (extractAll == true)
                                {
                                    if (isArchiveRoot)
                                    {
                                        //isArchiveRoot = dynamicNode.Tag.isArchiveRoot;
                                        currentArchiveName = dynamicNode.Name;
                                    }
                                }
                                else
                                {
                                    while (tmpNode != null)
                                    {
                                        if (tmpNode.Tag.isArchiveRoot == true)
                                        {
                                            currentArchiveName = tmpNode.Name;
                                            break;
                                        }
                                        tmpNode = tmpNode.Parent;
                                    }
                                }
                                if (!isArchiveRoot && dynamicNode.Tag.relativePath != null && dynamicNode.Tag.isDirectory != null)
                                    HelperService.selectedPaths.Add(new DDict(new Dictionary<string, dynamic>()
                                    {
                                        { "relativePath", dynamicNode.Tag.relativePath },
                                        { "fullPath", LongDirectory.Combine(tempDirectory, dynamicNode.Tag.relativePath) },
                                        { "isDirectory", dynamicNode.Tag.isDirectory },
                                        { "currentArchiveName", currentArchiveName }
                                    }));
                            }
                        }
                        catch { }

                        HelperService.selectedPaths = new ArrayList(HelperService.selectedPaths.ToArray().Distinct().ToArray());

                        if (HelperService.backgroundWorkerClosePending)
                            HelperService.selectedPaths = new ArrayList();

                        break;
                    }
            }
        }

        /********************************************************************************************************************/

        private void worker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage;
            // Set the text.
            this.Text = "Progress - " + e.ProgressPercentage.ToString() + "%";
            labelPercentage.Text = e.ProgressPercentage.ToString() + "%";
            labelResultMessage.Text = currentFileProcessed;

            TimeSpan timeDifference = currentDateTime.Subtract(startDateTime);
            StringBuilder elapsedTime = new StringBuilder();

            if (timeDifference.TotalSeconds > 0)
            {
                elapsedTime.Append("Elapsed time: ");
                if (timeDifference.Days > 0)
                {
                    elapsedTime.Append(timeDifference.Days + " days, " + timeDifference.Hours + " hours, " + timeDifference.Minutes + " minutes, ");
                }
                else
                if (timeDifference.Hours > 0)
                {
                    elapsedTime.Append(timeDifference.Hours + " hours, " + timeDifference.Minutes + " minutes, ");
                }
                else
                if (timeDifference.Minutes > 0) { elapsedTime.Append(timeDifference.Minutes + " minutes, "); }
                elapsedTime.Append(timeDifference.Seconds + " seconds");
            }
            else elapsedTime.Append("0 seconds");

            labelElapsedTime.Text = elapsedTime.ToString();
        }

        /********************************************************************************************************************/

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            workerIsDone = true;
            labelResultMessage.Text = resultMessage;
            
            if (progressBar1.Style == ProgressBarStyle.Marquee)
                progressBar1.MarqueeAnimationSpeed = 0;
            
            if (taskId == HelperService.ProcessingTask.InfoRegardingSelectedEntries)
                ShowInfoRegardingSelectedEntries();

            if (resultMessage != "")
            {
                btnClose.Visible = true;
                this.Size = new Size(380 * HelperService.scaling / 100, tableLayoutPanelProgress.Size.Height + 110 * HelperService.scaling / 100);
            }
            else this.Close();
        }

        /********************************************************************************************************************/

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (worker.IsBusy)
            {
                HelperService.backgroundWorkerClosePending = true;
                e.Cancel = true;
                resultMessage = "The task was canceled. You will see incomplete results.";
                return;
            }
            base.OnFormClosing(e);
        }

        /********************************************************************************************************************/

        private void ShowInfoRegardingSelectedEntries()
        {
            if (selectedNodesCount == 0)
                return;

            if (HelperService.cryptionSetting == HelperService.CryptionOptions.Encrypt)
            {
                if (selectedNodesCount == 1)
                {
                    MessageBox.Show(
                        $"Entry relative path: {relativePath}\r\n" +
                        $"Entry full path: {fullPath}\r\n" +
                        $"Entry type: {(clickedNode.ImageIndex == 1 ? "File" : "Directory")}\r\n" +
                        $"Uncompressed file size: {HelperService.FormattedBytesSize(uncompressedFileSize)}\r\n", "Entry Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Entry relative path: {relativePath} ... etc.\r\n" +
                        $"Entry full path: {fullPath} ... etc.\r\n" +
                        $"Entries type: Files and/or Directories\r\n" +
                        $"Nr. of entries: {selectedNodesCount}\r\n" +
                        $"Uncompressed files size: {HelperService.FormattedBytesSize(uncompressedFilesSize)}\r\n", "Entry Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (HelperService.cryptionSetting == HelperService.CryptionOptions.Decrypt)
            {
                long? cmprssdFullFileSize = null;
                long? cmprssdFullHeaderSize = null;
                bool isArchiveRoot = (dynamic)clickedNode.Tag == null;
                bool isSplitArchive = false;
                int splitCount = 0;

                try { cmprssdFullFileSize = ((dynamic)clickedNode.Tag).compressedFullFileSize; } catch { }
                try { cmprssdFullHeaderSize = ((dynamic)clickedNode.Tag).compressedFullHeaderSize; } catch { }
                try { isArchiveRoot = isArchiveRoot ? isArchiveRoot : ((dynamic)clickedNode.Tag).isArchiveRoot; } catch { }
                try { isSplitArchive = ((dynamic)clickedNode.Tag).isSplitArchive; } catch { }
                try { splitCount = ((dynamic)clickedNode.Tag).splitCount; } catch { }

                if (isArchiveRoot && selectedArchives == 1) // is archive
                {
                    MessageBox.Show($"Entry name: {clickedNode.Text}\r\n" +
                        $"Entry type: .NHC Archive\r\n" +
                        $"Is archive split: {(isSplitArchive ? "Yes" : "No")}\r\n" +
                        (isSplitArchive ? $"Split count: {splitCount}" : "") + "\r\n" +
                        $"Compression ratio: {(HelperService.uncompressedFilesSize == 0 ? "N/A" : (HelperService.archiveFileSize * 100 / HelperService.uncompressedFilesSize).ToString())}%\r\n" +
                        $"Archive size: {HelperService.FormattedBytesSize(HelperService.archiveFileSize)}\r\n" +
                        $"Uncompressed files size: {HelperService.FormattedBytesSize(HelperService.uncompressedFilesSize)}\r\n" +
                        $"Compressed files size: {HelperService.FormattedBytesSize(HelperService.compressedFilesSize)}\r\n" +
                        $"Compressed headers size: {HelperService.FormattedBytesSize(HelperService.compressedHeadersSize)}", "Entry Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (clickedNode.ImageIndex == 1 && selectedNodesCount == 1) // is file
                        MessageBox.Show($"Entry relative path: {((dynamic)clickedNode.Tag).relativePath}\r\n" +
                        $"Entry type: File\r\n" +
                        $"Compression ratio: {(((dynamic)clickedNode.Tag).uncompressedFileSize == 0 ? "N/A" : (((cmprssdFullFileSize ?? ((dynamic)clickedNode.Tag).compressedFileSize) + (cmprssdFullHeaderSize ?? ((dynamic)clickedNode.Tag).compressedHeaderSize)) * 100 / ((dynamic)clickedNode.Tag).uncompressedFileSize))}%\r\n" +
                        $"Uncompressed file size: {HelperService.FormattedBytesSize(((dynamic)clickedNode.Tag).uncompressedFileSize)}\r\n" +
                        $"Compressed file size: {HelperService.FormattedBytesSize(cmprssdFullFileSize ?? ((dynamic)clickedNode.Tag).compressedFileSize)}\r\n" +
                        $"Compressed header size: {HelperService.FormattedBytesSize(cmprssdFullHeaderSize ?? ((dynamic)clickedNode.Tag).compressedHeaderSize)}", "Entry Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else // is a combination of archives, directories and/or files
                {
                    MessageBox.Show($"{(isArchiveRoot ? ("Entry name: " + (dynamic)clickedNode.Text) : ("Entry relative path: " + ((dynamic)clickedNode.Tag).relativePath + (treeView1.SelectedNodes.Count == 1 ? "" : " ... etc.")))}\r\n" +
                        $"{(selectedNodesCount == 1 ? "Entry type: Directory" : ("Entries type: " + (selectedArchives > 0 ? "Archives, Files and/or Directories" : "Files and/or Directories")))}\r\n" +
                        $"Compression ratio: {((uncompressedFilesSize == 0) ? "N/A" : ((compressedFilesSize + compressedHeadersSize) * 100 / uncompressedFilesSize).ToString())}%\r\n" +
                        $"Nr. of entries selected: {selectedNodesCount}\r\n" +
                        $"Uncompressed files size: {HelperService.FormattedBytesSize(uncompressedFilesSize)}\r\n" +
                        $"Compressed files size: {HelperService.FormattedBytesSize(compressedFilesSize)}\r\n" +
                        $"Compressed headers size: {HelperService.FormattedBytesSize(selectedNodesCount > 1 ? compressedHeadersSize : (cmprssdFullHeaderSize ?? ((dynamic)clickedNode.Tag).compressedHeaderSize))}", "Entry Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /********************************************************************************************************************/

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /********************************************************************************************************************/

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
    }
}

/********************************************************************************************************************/
