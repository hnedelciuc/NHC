/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2020 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Helper Service.
/ Different helper methods used by other services.
/********************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Crypt;

internal class HelperService
{
    /********************************************************************************************************************/

    internal static ArrayList updatedImportedPaths = new ArrayList();
    internal static ArrayList importedPaths = new ArrayList();
    internal static ArrayList selectedPaths = new ArrayList();
    internal static bool preventCheck { get; set; }
    internal enum SelectionSetting { Checkboxes, Multiselect }
    internal static SelectionSetting selectionSetting { get; set; }
    internal enum ButtonSetting { IconsOnly, TextOnly, IconsAndText }
    internal static ButtonSetting buttonSetting { get; set; }
    internal enum WindowColorSetting { LightSteelBlue, DarkGray, White }
    internal static WindowColorSetting windowColorSetting { get; set; }
    internal enum PathModeSetting { EntireRelativePath, SelectedRelativePath }
    internal static PathModeSetting pathModeSetting { get; set; }
    internal enum CryptionOptions { Encrypt, Decrypt, Update };
    internal static CryptionOptions cryptionSetting { get; set; }
    internal enum CryptionAlgorithm
    {
        NeedleCryptKey,
        NeedleCryptPassword,
        RC2Key,
        RC2Password,
        TripleDesKey,
        TripleDesPassword,
        AesKey,
        AesPassword
    }
    internal static Dictionary<CryptionAlgorithm, string> cryptionAlgorithmDict = new Dictionary<CryptionAlgorithm, string>()
    {
        [CryptionAlgorithm.NeedleCryptKey] = "NeedleCrypt with Key",
        [CryptionAlgorithm.NeedleCryptPassword] = "NeedleCrypt with Key & Password",
        [CryptionAlgorithm.RC2Key] = "RC2 128bit with Key",
        [CryptionAlgorithm.RC2Password] = "RC2 128bit with Password",
        [CryptionAlgorithm.TripleDesKey] = "TripleDES 192bit with Key",
        [CryptionAlgorithm.TripleDesPassword] = "TripleDES 192bit with Password",
        [CryptionAlgorithm.AesKey] = "AES 256bit with Key",
        [CryptionAlgorithm.AesPassword] = "AES 256bit with Password",
    };
    internal static CryptionAlgorithm cryptionAlgorithm { get; set; }
    internal static CompressionLevel compressionLevel { get; set; }
    internal static bool forceLZMA { get; set; }
    internal static bool forceMAX { get; set; }
    internal enum OverwriteFilesSetting
    {
        Ask,
        Yes,
        No
    }
    internal enum ConfirmationButtons
    {
        OK,
        YesNo,
        YesNoCancel,
        YesToAllYesNoNoToAll,
        ShortcutGeneratorButtons
    }
    internal enum ProcessingTask
    {
        OpenArchive,
        ImportFilesDirectories_btnBrowseInput_Click,
        ImportFilesDirectories_treeView1_DragDrop,
        ImportFilesDirectories_LoadInputFiles,
        InfoRegardingSelectedEntries,
        DecompressDecryptOnlySelectedItems,
        UpdateArchiveContents
    }
    internal static Dictionary<string, Image> imageListDict = new Dictionary<string, Image>()
    {
        ["NHCFile"] = Crypt.Properties.Resources.Shortcut__3D__mini,                // Index = 0
        ["File"] = Crypt.Properties.Resources.Document_16x,                         // Index = 1
        ["FolderClosed"] = Crypt.Properties.Resources.FolderClosed_16x,             // Index = 2
        ["FolderOpened"] = Crypt.Properties.Resources.FolderOpened_16x,             // Index = 3
        ["HiddenFile"] = Crypt.Properties.Resources.HiddenFile_16x,                 // Index = 4
        ["HiddenFolderClosed"] = Crypt.Properties.Resources.HiddenFolderClosed_16x, // Index = 5
        ["HiddenFolderOpened"] = Crypt.Properties.Resources.HiddenFolderOpened_16x, // Index = 6
        ["DocumentsFolder"] = Crypt.Properties.Resources.DocumentsFolder_16x,       // Index = 7
        ["ImageFolder"] = Crypt.Properties.Resources.Image_16x,                     // Index = 8
        ["VideoFolder"] = Crypt.Properties.Resources.VideoCamera_16x,               // Index = 9
        ["AudioFolder"] = Crypt.Properties.Resources.Audio_16x,                     // Index = 10
        ["ZipFile"] = Crypt.Properties.Resources.ZipFile_16x,                       // Index = 11
        ["HardDrive"] = Crypt.Properties.Resources.HardDrive_16x,                   // Index = 12
        ["CDDrive"] = Crypt.Properties.Resources.CDDrive_16x,                       // Index = 13
        ["Network"] = Crypt.Properties.Resources.Network_16x                        // Index = 14
    };
    internal static TreeNode[] rootNodes;
    internal static ArrayList argPaths = new ArrayList();
    internal static string keyFileName;
    internal static string pwd;
    internal static string output;
    internal static long archiveFileSize = 0;
    internal static long compressedFilesSize = 0;
    internal static long uncompressedFilesSize = 0;
    internal static long compressedHeadersSize = 0;
    internal static long splitCount = 0;
    internal static int scaling { get; set; }
    internal static bool backgroundWorkerClosePending;
    internal static bool exitOnClose = false;

    /********************************************************************************************************************/

    internal static bool IsKeyBasedCryptionAlgorithm()
    {
        switch (cryptionAlgorithm)
        {
            case CryptionAlgorithm.AesKey:
            case CryptionAlgorithm.RC2Key:
            case CryptionAlgorithm.TripleDesKey:
            case CryptionAlgorithm.NeedleCryptKey:
            case CryptionAlgorithm.NeedleCryptPassword:
                return true;
            default: 
                return false;
        }
    }

    /********************************************************************************************************************/

    internal static bool IsPasswordBasedCryptionAlgorithm()
    {
        switch (cryptionAlgorithm)
        {
            case CryptionAlgorithm.AesPassword:
            case CryptionAlgorithm.RC2Password:
            case CryptionAlgorithm.TripleDesPassword:
            case CryptionAlgorithm.NeedleCryptPassword:
                return true;
            default:
                return false;
        }
    }

    /********************************************************************************************************************/

    internal static void ListDirectory(TreeViewMS treeView, string path)
    {
        StringBuilder errors = new StringBuilder();
        path = path.TrimEnd('\\');
        var stack = new Stack<TreeNode>();
        var rootDirectoryName = LongDirectory.GetJustDirectoryName(path).TrimStart('\\');
        rootDirectoryName = rootDirectoryName == string.Empty ? path.Replace(":\\", "Drive") : rootDirectoryName;
        var rootNode = new TreeNode(rootDirectoryName) { Name = path, ImageIndex = 2, SelectedImageIndex = 2, Tag = new { relativePath = rootDirectoryName } };
        stack.Push(rootNode);

        while (stack.Count > 0)
        {
            if (HelperService.backgroundWorkerClosePending)
                break;

            var currentNode = stack.Pop();
            string[] directories = null;
            string[] files = null;

            try
            {
                directories = LongDirectory.GetDirectories(currentNode.Name);
                files = LongDirectory.GetFiles(currentNode.Name);

                if (directories != null && files != null)
                {
                    foreach (var directory in directories)
                    {
                        if (HelperService.backgroundWorkerClosePending)
                            break;

                        string relativePath = LongDirectory.Combine(rootDirectoryName, LongDirectory.TrimPath(directory.Substring(path.Length)));
                        object pathObj = new { relativePath, fullPath = directory, isDirectory = true, uncompressedFileSize = 0 };
                        if (!importedPaths.Contains(pathObj))
                        {
                            importedPaths.Add(pathObj);
                        }
                        var childDirectoryNode = new TreeNode(LongDirectory.GetJustDirectoryName(directory)) { Name = directory, ImageIndex = 2, SelectedImageIndex = 2, Tag = new { relativePath } };
                        currentNode.Nodes.Add(childDirectoryNode);
                        stack.Push(childDirectoryNode);
                    }
                    foreach (var file in files)
                    {
                        if (HelperService.backgroundWorkerClosePending)
                            break;

                        string relativePath = LongDirectory.Combine(rootDirectoryName,
                            ((LongDirectory.TrimPath(file.Substring(path.Length)) == String.Empty) ? "" :
                            LongDirectory.TrimPath(file.Substring(path.Length))));

                        using (var handle = LongFile.GetFileHandle(LongFile.GetWin32LongPath(file)))
                        using (var stream = new FileStream(handle, FileAccess.Read))
                        {
                            var uncompressedFileSize = stream.Length;

                            object pathObj = new { relativePath, fullPath = file, isDirectory = false, uncompressedFileSize };
                            if (!importedPaths.Contains(pathObj))
                            {
                                importedPaths.Add(pathObj);
                            }

                            currentNode.Nodes.Add(new TreeNode(Path.GetFileName(file)) { ImageIndex = 1, SelectedImageIndex = 1, Tag = new { relativePath } });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Append("\r\n\r\n" + currentNode.Name + "\r\n\r\n" + ex.Message);
            }
        }

        if (!treeView.Nodes.ContainsKey(rootNode.Name))
        {
            rootNode.Tag = new { relativePath = rootNode.Text };
            treeView.Nodes.Add(rootNode);
            importedPaths.Add(new { relativePath = rootNode.Text, fullPath = rootNode.Name, isDirectory = true, uncompressedFileSize = 0 });
        }
        else
        {
            treeView.SelectedNodes = new ArrayList();
            var index = treeView.Nodes.IndexOfKey(rootNode.Name);
            TreeNode[] newChildNodes = new TreeNode[rootNode.Nodes.Count];
            for (var i = 0; i < rootNode.Nodes.Count; i++)
            {
                newChildNodes[i] = rootNode.Nodes[i];
            }
            treeView.Nodes[index].Nodes.Clear();
            treeView.Nodes[index].Nodes.AddRange(newChildNodes);
        }

        if (errors.Length > 0)
            MessageBox.Show("There were errors accessing the following files/folders:" + errors.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /********************************************************************************************************************/

    internal static void RemoveSelectedPaths(ArrayList selectedPaths)
    {
        ArrayList selectedPathsTemp = new ArrayList();

        foreach (var p in HelperService.importedPaths)
        {
            selectedPathsTemp.Add(p);
        }

        foreach (string path in selectedPaths)
        {
            foreach (dynamic p in selectedPathsTemp)
            {
                if ((p.relativePath == "" && p.fullPath.Substring(p.fullPath.LastIndexOf("\\") + 1) == path) || (p.relativePath == path || p.relativePath.IndexOf(path + "\\") != -1))
                {
                    HelperService.importedPaths.Remove(p);
                }
            }
        }
    }

    /********************************************************************************************************************/

    internal static void RemoveSelectedNodes(TreeNodeCollection nodes, ArrayList selectedPaths)
    {
        List<TreeNode> selectedNodes = new List<TreeNode>();

        foreach (TreeNode node in nodes)
        {
            if (selectedPaths.Contains(node.FullPath))
            {
                selectedNodes.Add(node);
            }
            else
            {
                RemoveSelectedNodes(node.Nodes, selectedPaths);
            }
        }

        foreach (TreeNode selectedNode in selectedNodes)
        {
            try
            {
                nodes.Remove(selectedNode);
            }
            catch { }
        }
    }

    /********************************************************************************************************************/

    internal static void SelectAllNodes(TreeNodeCollection nodes, bool isSelected, ref ArrayList allNodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (isSelected) allNodes.Add(node);
            else allNodes.Remove(node);

            SelectAllNodes(node.Nodes, isSelected, ref allNodes);
        }
    }

    /********************************************************************************************************************/

    internal static void CheckAllNodes(TreeNodeCollection nodes, bool isChecked)
    {
        foreach (TreeNode node in nodes)
        {
            if (isChecked) node.Checked = true;
            else node.Checked = false;

            CheckAllNodes(node.Nodes, isChecked);
        }
    }

    /********************************************************************************************************************/

    internal static void CheckParentNodes(TreeNodeCollection nodes)
    {
        int countNodes = 0;
        foreach (TreeNode node in nodes)
        {
            if (node.Checked)
                countNodes++;
        }
        if (countNodes < nodes.Count)
        {
            if (nodes[0].Parent != null)
            {
                preventCheck = true;
                nodes[0].Parent.Checked = false;
                preventCheck = false;
                if (nodes[0].Parent.Parent != null)
                    CheckParentNodes(nodes[0].Parent.Parent.Nodes);
            }
        } else
        // if countNodes == nodes.Count...
        {
            if (nodes[0].Parent != null)
            {
                preventCheck = true;
                nodes[0].Parent.Checked = true;
                preventCheck = false;
                if (nodes[0].Parent.Parent != null)
                    CheckParentNodes(nodes[0].Parent.Parent.Nodes);
            }
        }
        
    }

    /********************************************************************************************************************/

    internal static void GetCheckedNodes(TreeNodeCollection nodes, ref ArrayList selectedNodes, bool includeSubNodes = false)
    {
        foreach (TreeNode node in nodes)
        {
            if (HelperService.backgroundWorkerClosePending)
                break;

                bool isParentNodeChecked = node.Parent != null ? node.Parent.Checked : false;
            if (node.Checked && node.Text != "" && (includeSubNodes ? true : !isParentNodeChecked))
                selectedNodes.Add(node);

            if (node.Nodes.Count > 0)
                GetCheckedNodes(node.Nodes, ref selectedNodes, includeSubNodes);
        }
    }

    /********************************************************************************************************************/

    internal static void GetSelectedNodes(ArrayList nodes, ref ArrayList selectedNodes)
    {
        foreach (TreeNode node in nodes)
        {
            if (HelperService.backgroundWorkerClosePending)
                break;

            selectedNodes.Add(node);

            if (node.Nodes.Count > 0)
                GetSelectedNodes(new ArrayList(node.Nodes), ref selectedNodes);
        }
    }

    /********************************************************************************************************************/

    internal static void AddNodePath(TreeNode node, string previousPath, string remainingPath, long fileSize, long compressedFileSize, int compressedHeaderSize, bool isDirectory, ref bool isSplitContinuation)
    {
        if (remainingPath.Length > 0)
        {
            var pathArray = LongDirectory.TrimPath(remainingPath).Split('\\');
            var currentPath = LongDirectory.TrimPath(previousPath + "\\" + pathArray[0]);
            int remainingIndex = 0;
            remainingIndex = remainingPath.IndexOf("\\");
            remainingPath = remainingIndex != -1 ? remainingPath.Substring(remainingIndex + 1) : String.Empty;
            if (node.Nodes.ContainsKey(currentPath))
            {
                if (!isDirectory && remainingPath == String.Empty)
                {
                    var imageIndex = pathArray.Length > 1 ? 2 : isDirectory ? 2 : 1;
                    dynamic tag = node.Nodes[node.Nodes.IndexOfKey(currentPath)].Tag;
                    
                    long? cmprssdFullFileSize = null;
                    int? cmprssdFullHeaderSize = null;

                    try
                    {
                        cmprssdFullFileSize = tag.compressedFullFileSize;
                        cmprssdFullHeaderSize = tag.compressedFullHeaderSize;
                    }
                    catch { }

                    cmprssdFullFileSize = cmprssdFullFileSize == null ? (long)tag.compressedFileSize + compressedFileSize : (long)cmprssdFullFileSize + compressedFileSize;
                    cmprssdFullHeaderSize = cmprssdFullHeaderSize == null ? (int)tag.compressedHeaderSize + compressedHeaderSize : (int)cmprssdFullHeaderSize + compressedHeaderSize;

                    if (tag.uncompressedFileSize != fileSize) { throw new Exception("tag.uncompressedFileSize != fileSize"); }

                    node.Nodes[node.Nodes.IndexOfKey(currentPath)].Tag = new
                    {
                        compressedFileSize = imageIndex != 1 ? 0 : compressedFileSize,
                        compressedFullFileSize = imageIndex != 1 ? 0 : (long)cmprssdFullFileSize,
                        uncompressedFileSize = imageIndex != 1 ? 0 : fileSize,
                        compressedHeaderSize = imageIndex == 1 || isDirectory ? compressedHeaderSize : 0,
                        compressedFullHeaderSize = imageIndex == 1 || isDirectory ? (int)cmprssdFullHeaderSize : 0,
                        relativePath = currentPath,
                        isDirectory = imageIndex == 2
                    };

                    isSplitContinuation = true;
                }
                else AddNodePath(node.Nodes[node.Nodes.IndexOfKey(currentPath)], currentPath, remainingPath, fileSize, compressedFileSize, compressedHeaderSize, isDirectory, ref isSplitContinuation);
            }
            else
            {
                var imageIndex = pathArray.Length > 1 ? 2 : isDirectory ? 2 : 1;
                var selectedImageIndex = pathArray.Length > 1 ? 2 : isDirectory ? 2 : 1;
                node.Nodes.Add(new TreeNode(pathArray[0])
                {
                    Name = currentPath,
                    ImageIndex = imageIndex,
                    SelectedImageIndex = selectedImageIndex,
                    Tag = new
                    {
                        compressedFileSize = imageIndex != 1 ? 0 : compressedFileSize,
                        uncompressedFileSize = imageIndex != 1 ? 0 : fileSize,
                        compressedHeaderSize = imageIndex == 1 || isDirectory ? compressedHeaderSize : 0,
                        relativePath = currentPath,
                        isDirectory = imageIndex == 2
                    }
                });
                AddNodePath(node.Nodes[node.Nodes.Count - 1], currentPath, remainingPath, fileSize, compressedFileSize, compressedHeaderSize, isDirectory, ref isSplitContinuation);
            }
        }
    }

    /********************************************************************************************************************/

    internal class NodeSorter : System.Collections.IComparer
    {
        internal NodeSorter(bool doNotSortFirstLevel = false)
        {
            isFirstLevelSorted = !doNotSortFirstLevel;
        }
        private bool isFirstLevelSorted { get; set; }
        public int Compare(object x, object y)
        {
            TreeNode tx = x as TreeNode;
            TreeNode ty = y as TreeNode;

            if (!isFirstLevelSorted && tx.Level == 0 && ty.Level == 0)
            {
                return 0;
            }

            string s1 = tx.Text;
            string s2 = ty.Text;

            int nameComparison = NativeMethods.StrCmpLogicalW(s1, s2);
            int directoryComparison = CompareByDirectory(tx, ty);

            if (nameComparison == -1 && directoryComparison == -1)
                return -1;
            if (nameComparison == 0 && directoryComparison == -1)
                return -1;
            if (nameComparison == 1 && directoryComparison == -1)
                return -1;
            if (nameComparison == -1 && directoryComparison == 0)
                return -1;
            if (nameComparison == 0 && directoryComparison == 0)
                return 0;
            if (nameComparison == 1 && directoryComparison == 0)
                return 1;
            if (nameComparison == -1 && directoryComparison == 1)
                return 1;
            if (nameComparison == 0 && directoryComparison == 1)
                return 1;
            if (nameComparison == 1 && directoryComparison == 1)
                return 1;
            throw new Exception();
        }

        private int CompareByDirectory(TreeNode tx, TreeNode ty)
        {
            bool txIsDirectory = tx.ImageIndex == 0 || tx.ImageIndex == 2 || tx.ImageIndex == 3;
            bool tyIsDirectory = ty.ImageIndex == 0 || ty.ImageIndex == 2 || ty.ImageIndex == 3;

            if (txIsDirectory && tyIsDirectory)
                return 0;
            if (!txIsDirectory && !tyIsDirectory)
                return 0;
            if (txIsDirectory && !tyIsDirectory)
                return -1;
            if (!txIsDirectory && tyIsDirectory)
                return 1;
            throw new Exception();
        }
    }

    /********************************************************************************************************************/

    /// <summary>
    /// This class suppresses stack walks for unmanaged code permission. 
    /// (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
    /// This class is for methods that are safe for anyone to call. 
    /// Callers of these methods are not required to perform a full security review to make sure that the 
    /// usage is secure because the methods are harmless for any caller.
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("shlwapi.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        internal static extern Int32 StrCmpLogicalW(string psz1, string psz2);

        [DllImport("shell32.dll")]
        internal static extern void SHChangeNotify(HChangeNotifyEventID wEventId,
                                           HChangeNotifyFlags uFlags,
                                           IntPtr dwItem1,
                                           IntPtr dwItem2);

        /// <summary>
        /// Describes the event that has occurred. 
        /// Typically, only one event is specified at a time. 
        /// If more than one event is specified, the values contained 
        /// in the <i>dwItem1</i> and <i>dwItem2</i> 
        /// parameters must be the same, respectively, for all specified events. 
        /// This parameter can be one or more of the following values. 
        /// </summary>
        /// <remarks>
        /// <para><b>Windows NT/2000/XP:</b> <i>dwItem2</i> contains the index 
        /// in the system image list that has changed. 
        /// <i>dwItem1</i> is not used and should be <see langword="null"/>.</para>
        /// <para><b>Windows 95/98:</b> <i>dwItem1</i> contains the index 
        /// in the system image list that has changed. 
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>.</para>
        /// </remarks>
        [Flags]
        internal enum HChangeNotifyEventID
        {
            /// <summary>
            /// All events have occurred. 
            /// </summary>
            SHCNE_ALLEVENTS = 0x7FFFFFFF,

            /// <summary>
            /// A file type association has changed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> 
            /// must be specified in the <i>uFlags</i> parameter. 
            /// <i>dwItem1</i> and <i>dwItem2</i> are not used and must be <see langword="null"/>. 
            /// </summary>
            SHCNE_ASSOCCHANGED = 0x08000000,

            /// <summary>
            /// The attributes of an item or folder have changed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the item or folder that has changed. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_ATTRIBUTES = 0x00000800,

            /// <summary>
            /// A nonfolder item has been created. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the item that was created. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_CREATE = 0x00000002,

            /// <summary>
            /// A nonfolder item has been deleted. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the item that was deleted. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_DELETE = 0x00000004,

            /// <summary>
            /// A drive has been added. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive that was added. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_DRIVEADD = 0x00000100,

            /// <summary>
            /// A drive has been added and the Shell should create a new window for the drive. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive that was added. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_DRIVEADDGUI = 0x00010000,

            /// <summary>
            /// A drive has been removed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive that was removed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_DRIVEREMOVED = 0x00000080,

            /// <summary>
            /// Not currently used. 
            /// </summary>
            SHCNE_EXTENDED_EVENT = 0x04000000,

            /// <summary>
            /// The amount of free space on a drive has changed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive on which the free space changed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_FREESPACE = 0x00040000,

            /// <summary>
            /// Storage media has been inserted into a drive. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive that contains the new media. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_MEDIAINSERTED = 0x00000020,

            /// <summary>
            /// Storage media has been removed from a drive. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the root of the drive from which the media was removed. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_MEDIAREMOVED = 0x00000040,

            /// <summary>
            /// A folder has been created. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> 
            /// or <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the folder that was created. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_MKDIR = 0x00000008,

            /// <summary>
            /// A folder on the local computer is being shared via the network. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the folder that is being shared. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_NETSHARE = 0x00000200,

            /// <summary>
            /// A folder on the local computer is no longer being shared via the network. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the folder that is no longer being shared. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_NETUNSHARE = 0x00000400,

            /// <summary>
            /// The name of a folder has changed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the previous pointer to an item identifier list (PIDL) or name of the folder. 
            /// <i>dwItem2</i> contains the new PIDL or name of the folder. 
            /// </summary>
            SHCNE_RENAMEFOLDER = 0x00020000,

            /// <summary>
            /// The name of a nonfolder item has changed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the previous PIDL or name of the item. 
            /// <i>dwItem2</i> contains the new PIDL or name of the item. 
            /// </summary>
            SHCNE_RENAMEITEM = 0x00000001,

            /// <summary>
            /// A folder has been removed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the folder that was removed. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_RMDIR = 0x00000010,

            /// <summary>
            /// The computer has disconnected from a server. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the server from which the computer was disconnected. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// </summary>
            SHCNE_SERVERDISCONNECT = 0x00004000,

            /// <summary>
            /// The contents of an existing folder have changed, 
            /// but the folder still exists and has not been renamed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or 
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>. 
            /// <i>dwItem1</i> contains the folder that has changed. 
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>. 
            /// If a folder has been created, deleted, or renamed, use SHCNE_MKDIR, SHCNE_RMDIR, or 
            /// SHCNE_RENAMEFOLDER, respectively, instead. 
            /// </summary>
            SHCNE_UPDATEDIR = 0x00001000,

            /// <summary>
            /// An image in the system image list has changed. 
            /// <see cref="HChangeNotifyFlags.SHCNF_DWORD"/> must be specified in <i>uFlags</i>. 
            /// </summary>
            SHCNE_UPDATEIMAGE = 0x00008000,
        }

        /// <summary>
        /// Flags that indicate the meaning of the <i>dwItem1</i> and <i>dwItem2</i> parameters. 
        /// The uFlags parameter must be one of the following values.
        /// </summary>
        [Flags]
        internal enum HChangeNotifyFlags
        {
            /// <summary>
            /// The <i>dwItem1</i> and <i>dwItem2</i> parameters are DWORD values. 
            /// </summary>
            SHCNF_DWORD = 0x0003,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of ITEMIDLIST structures that 
            /// represent the item(s) affected by the change. 
            /// Each ITEMIDLIST must be relative to the desktop folder. 
            /// </summary>
            SHCNF_IDLIST = 0x0000,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of 
            /// maximum length MAX_PATH that contain the full path names 
            /// of the items affected by the change. 
            /// </summary>
            SHCNF_PATHA = 0x0001,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of 
            /// maximum length MAX_PATH that contain the full path names 
            /// of the items affected by the change. 
            /// </summary>
            SHCNF_PATHW = 0x0005,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that 
            /// represent the friendly names of the printer(s) affected by the change. 
            /// </summary>
            SHCNF_PRINTERA = 0x0002,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that 
            /// represent the friendly names of the printer(s) affected by the change. 
            /// </summary>
            SHCNF_PRINTERW = 0x0006,
            /// <summary>
            /// The function should not return until the notification 
            /// has been delivered to all affected components. 
            /// As this flag modifies other data-type flags, it cannot by used by itself.
            /// </summary>
            SHCNF_FLUSH = 0x1000,
            /// <summary>
            /// The function should begin delivering notifications to all affected components 
            /// but should return as soon as the notification process has begun. 
            /// As this flag modifies other data-type flags, it cannot by used by itself.
            /// </summary>
            SHCNF_FLUSHNOWAIT = 0x2000
        }
    }

    /********************************************************************************************************************/

    private static void RestartWithAdminUserPrivileges(string args)
    {
        Process process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = Application.ExecutablePath,
            UseShellExecute = true,
            Verb = "runas",
            Arguments = args
        };
        process.Start();
        exitOnClose = true;
        Application.Exit();
    }

    /********************************************************************************************************************/

    private static bool IsCurrentUserWithAdminPrivileges()
    {
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    /********************************************************************************************************************/

    internal static string FormattedBytesSize(long n)
    {
        StringBuilder sb = new StringBuilder();

        string nStr = n.ToString();
        
        for (int i = nStr.Length - 1; i >= 0; i--)
        {
            if ((nStr.Length - i) % 3 == 0)
            {
                sb.Append(nStr[i]);
                if (i > 0) sb.Append(",");
            }
            else sb.Append(nStr[i]);
        }
        return new String(sb.ToString().Reverse().ToArray()) + " bytes";
    }

    /********************************************************************************************************************/

    // Returns integer with size (in bytes) of a given byte array header.

    internal static int GetCompressedHeaderSize(byte[] decompressedHeader)
    {
        byte[] compressedHeader = CompressionService.DeflateCompressHeader(decompressedHeader);
        return compressedHeader.Length;
    }

    /********************************************************************************************************************/

    // Trims byte array of all trailing bytes with value = 0. Returns trimmed byte array.

    internal static byte[] TrimByteArray(byte[] array)
    {
        int pos = array.Length - 1;
        byte b = array[pos];

        // Iterate from end of "array" towards the index position "pos" where data begins.
        // Bytes with value = 0 are ignored and stepped over, until non-0 element is found.

        while ((b == 0) && (pos > 0))
        {
            pos--;
            b = array[pos];
        }

        // Copy all data from "array" to "result" except trailing 0 bytes.

        byte[] result = new byte[pos + 1];

        for (int i = 0; i <= pos; i++)
        {
            result[i] = array[i];
        }

        return result;
    }

    /********************************************************************************************************************/

    // Pads byte array at the end with bytes with value = 0 up to desired size. Returns padded byte array.

    internal static byte[] PadByteArray(byte[] array, int desiredSize)
    {
        byte[] result = new byte[desiredSize];

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i];
        }

        // Since all the elements in the "result" array up to "desiredSize" were automatically initialized as 0,
        // there is no need to iterate and assign the value 0 to elements with index >= than "array.Length",
        // since the "result" array already comes out padded with 0's.

        return result;
    }

    /********************************************************************************************************************/

    internal static byte[] CombineByteArrays(params byte[][] arrays)
    {
        byte[] result = new byte[arrays.Sum(a => a.Length)];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }
        return result;
    }

    /********************************************************************************************************************/

    internal static byte[] GetByteArraySegment(byte[] arr, int offset, int? count = null)
    {
        if (count == null) { count = arr.Length - offset; }
        return new ArraySegment<byte>(arr, offset, count.Value).ToArray();
    }

    /********************************************************************************************************************/

    internal static Tuple<byte[], byte[]> SplitByteArray(byte[] arr, int offset)
    {
        return new Tuple<byte[], byte[]>(GetByteArraySegment(arr, 0, offset), GetByteArraySegment(arr, offset));
    }

    /********************************************************************************************************************/

    internal static byte ConvertBoolArrayToByte(bool[] source)
    {
        if (source == null)
            throw new ArgumentNullException();
        if (source.Length != 8)
            throw new ArgumentException("\"source\" array length should equal 8.");

        byte result = 0;
        int index = 8 - source.Length;

        foreach (bool b in source)
        {
            // if the element is 'true' set the bit at that position
            if (b)
                result |= (byte)(1 << (7 - index));

            index++;
        }

        return result;
    }

    /********************************************************************************************************************/

    internal static bool[] ConvertByteToBoolArray(byte b)
    {
        bool[] result = new bool[8];

        // check each bit in the byte. if 1 set to true, if 0 set to false
        for (int i = 0; i < 8; i++)
            result[i] = (b & (1 << i)) == 0 ? false : true;

        // reverse the array
        Array.Reverse(result);

        return result;
    }

    /********************************************************************************************************************/

    // Generates and returns random byte.

    private static Random rnd = new Random();

    internal static byte RandomByte()
    {
        return (byte)rnd.Next(256);
    }

    /********************************************************************************************************************/

    // Checks if pair (index, element) and (element, index) is available (i.e. null) to be added in the given cryption key array.

    internal static bool ElementAvailable(byte?[] byteArray, byte index, byte element)
    {
        if (byteArray[index] == null && byteArray[element] == null)
        {
            return true;
        }

        return false;
    }

    /********************************************************************************************************************/

    // Checks cryption key array for null elements.
    // If result = false, the cryption key array is full (i.e. cryption key is done).

    internal static bool CheckNull(byte?[] byteArray)
    {
        for (int i = 0; i <= 255; i++)
        {
            if (byteArray[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    /********************************************************************************************************************/

    internal static string[] ToStringArray(object[] array)
    {
        string[] result = new string[array.Length];

        for (var i = 0; i < array.Length; i++)
        {
            result[i] = (string)array[i];
        }

        return result;
    }

    /********************************************************************************************************************/

    internal static int FindLongestLengthOfElementText(Menu.MenuItemCollection collection)
    {
        int longestLength = 0;

        foreach (MenuItem elem in collection)
        {
            if (elem.Visible && elem.Text.Length > longestLength)
            {
                longestLength = elem.Text.Length;
            }
        }

        return longestLength;
    }

    /********************************************************************************************************************/

    internal static int FindImageIndexByKey(string key)
    {
        for (var i = 0; i < imageListDict.Count; i++)
        {
            if (imageListDict.ElementAt(i).Key == key)
            {
                return i;
            }
        }
        throw new ArgumentException("There is no ImageKey with this name: " + key);
    }

    /********************************************************************************************************************/

    internal static void InitializeImageList(ImageList imageList)
    {
        int height = imageList.ImageSize.Height * scaling / 100;
        int width = imageList.ImageSize.Width * scaling / 100;
        imageList.ImageSize = new Size(height > 256 ? 256 : height, width > 256 ? 256 : width);
        imageList.ColorDepth = ColorDepth.Depth32Bit;
        imageList.TransparentColor = Color.Transparent;
        imageList.Images.Clear();
        for (var i = 0; i < imageListDict.Count; i++)
        {
            var img = imageListDict.ElementAt(i);
            imageList.Images.Add(img.Key, img.Value);
        }
    }

    /********************************************************************************************************************/

    internal static void CreateShortcut(string path)
    {
        WshShell shell = new WshShell();
        //object shDesktop = (object)"Desktop";
        //string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Crypt.lnk";
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(path);
        shortcut.Description = "Needle in a Haystack in a Crypt";
        //shortcut.Hotkey = "Ctrl+Shift+N";
        shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
        shortcut.Arguments = SaveArgs();
        shortcut.Save();
    }

    /********************************************************************************************************************/

    internal static void CreateFileAssociation(bool isAfterRestart = false)
    {
        if (IsCurrentUserWithAdminPrivileges())
        {
            SetFileAssociation(
                    Application.ExecutablePath,                //Application exe path
                    "nhc",                                     //Document file extension
                    "Needle in a Haystack in a Crypt Archive", //Document type description
                    Application.ExecutablePath + ",0",         //Document type icon
                    "Open NHC Archive",                        //Action name
                    "open"                                     //File command
                );
        }
        else if (!isAfterRestart)
        {
            MessageBox.Show("In order to create file associations for .NHC archives the application will restart with Administrator privileges. Please accept request when prompted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            RestartWithAdminUserPrivileges($"-setFileAssociation {SaveArgsAll()}");
        }
    }

    /********************************************************************************************************************/

    private static void SetFileAssociation(string ExePath, string FileExt, string Description, string Icon, string ActionName, string FileCommand)
    {
        try
        {
            FileExt = FileExt.StartsWith(".") ? FileExt.Substring(1) : FileExt;
            RegistryKey RegKey = Registry.ClassesRoot.CreateSubKey("." + FileExt);
            RegKey.SetValue("", FileExt + "File");
            RegKey = Registry.ClassesRoot.CreateSubKey(FileExt + "File");
            RegKey.SetValue("", Description);
            RegKey = Registry.ClassesRoot.CreateSubKey(FileExt + @"File\DefaultIcon");
            RegKey.SetValue("", Icon, RegistryValueKind.ExpandString);
            RegKey = Registry.ClassesRoot.CreateSubKey(FileExt + @"File\shell");
            RegKey.SetValue("", FileCommand);
            RegKey = Registry.ClassesRoot.CreateSubKey(FileExt + @"File\shell\" + FileCommand);
            RegKey.SetValue("", ActionName);
            RegKey = Registry.ClassesRoot.CreateSubKey(FileExt + @"File\shell\" + FileCommand + @"\command");
            RegKey.SetValue("", "\"" + ExePath + "\" \"%1\"");
            RegKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\." + FileExt, true);
            RegKey?.DeleteSubKey("UserChoice", false);
            RegKey?.Close();
            NativeMethods.SHChangeNotify(NativeMethods.HChangeNotifyEventID.SHCNE_ASSOCCHANGED, NativeMethods.HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error creating file association: " + ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : string.Empty), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /********************************************************************************************************************/

    internal static void CreateSendToShortcut()
    {
        try
        {
            CreateShortcut(LongDirectory.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Windows\SendTo\Needle in a Haystack in a Crypt.lnk"));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error creating 'Send to...' shortcut: " + 
                Environment.NewLine + Environment.NewLine + ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : string.Empty),
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /********************************************************************************************************************/

    internal static string SaveArgs()
    {
        string args = String.Empty;

        switch (buttonSetting)
        {
            case ButtonSetting.IconsOnly: args += " -useIconsOnly"; break;
            case ButtonSetting.TextOnly: args += " -useTextOnly"; break;
            case ButtonSetting.IconsAndText: args += " -useIconsAndText"; break;
            default: break;
        }

        switch (selectionSetting)
        {
            case SelectionSetting.Checkboxes: args += " -useCheckboxes"; break;
            case SelectionSetting.Multiselect: args += " -useMultipleSelection"; break;
            default: break;
        }
        
        switch (windowColorSetting)
        {
            case WindowColorSetting.LightSteelBlue: args += " -windowColorLightSteelBlue"; break;
            case WindowColorSetting.DarkGray: args += " -windowColorDarkGray"; break;
            case WindowColorSetting.White: args += " -windowColorWhite"; break;
        }

        switch (pathModeSetting)
        {
            case PathModeSetting.EntireRelativePath: args += " -useEntireRelativePath"; break;
            case PathModeSetting.SelectedRelativePath: args += " -useSelectedRelativePath"; break;
        }

        switch (HelperService.compressionLevel)
        {
            case CompressionLevel.Optimal: args += " -useHighCompression"; break;
            case CompressionLevel.Fastest: args += " -useFastCompression"; break;
            case CompressionLevel.NoCompression: args += " -useNoCompression"; break;
        }

        if (forceMAX)
            args += " -forceMAX";
        else
        if (forceLZMA)
            args += " -forceLZMA";

        return args.Trim();
    }

    /********************************************************************************************************************/

    internal static string SaveArgsAll()
    {
        StringBuilder sb = new StringBuilder();

        switch (HelperService.cryptionSetting)
        {
            case CryptionOptions.Encrypt: sb.Append(" -encrypt"); break;
            case CryptionOptions.Decrypt: sb.Append(" -decrypt"); break;
        }

        switch (HelperService.cryptionAlgorithm)
        {
            case CryptionAlgorithm.NeedleCryptKey: sb.Append(" -useNeedleCryptKey"); break;
            case CryptionAlgorithm.NeedleCryptPassword: sb.Append(" -useNeedleCryptPwd"); break;
            case CryptionAlgorithm.RC2Key: sb.Append(" -useRC2Key"); break;
            case CryptionAlgorithm.RC2Password: sb.Append(" -useRC2Pwd"); break;
            case CryptionAlgorithm.TripleDesKey: sb.Append(" -useTripleDESKey"); break;
            case CryptionAlgorithm.TripleDesPassword: sb.Append(" -useTripleDESPwd"); break;
            case CryptionAlgorithm.AesKey: sb.Append(" -useAESKey"); break;
            case CryptionAlgorithm.AesPassword: sb.Append(" -useAESPwd"); break;
        }

        sb.Append(" \"-password:" + HelperService.pwd + "\"");
        sb.Append(" \"-keyFilePath:" + HelperService.keyFileName + "\"");
        sb.Append(" \"-output:" + HelperService.output + "\"");

        switch (HelperService.compressionLevel)
        {
            case CompressionLevel.Optimal: sb.Append(" -useHighCompression");  break;
            case CompressionLevel.Fastest: sb.Append(" -useFastCompression"); break;
            case CompressionLevel.NoCompression: sb.Append(" -useNoCompression"); break;
        }

        if (forceMAX)
            sb.Append(" -forceMAX");
        else
        if (forceLZMA)
            sb.Append(" -forceLZMA");

        switch (HelperService.buttonSetting) {
            case ButtonSetting.IconsOnly: sb.Append(" -useIconsOnly"); break;
            case ButtonSetting.TextOnly: sb.Append(" -useTextOnly"); break;
            case ButtonSetting.IconsAndText: sb.Append(" -useIconsAndText"); break;
        }

        switch (HelperService.selectionSetting)
        {
            case SelectionSetting.Checkboxes: sb.Append(" -useCheckboxes"); break;
            case SelectionSetting.Multiselect: sb.Append(" -useMultipleSelection"); break;
        }

        switch (HelperService.windowColorSetting)
        {
            case WindowColorSetting.LightSteelBlue: sb.Append(" -windowColorLightSteelBlue"); break;
            case WindowColorSetting.DarkGray: sb.Append(" -windowColorDarkGray"); break;
            case WindowColorSetting.White: sb.Append(" -windowColorWhite"); break;
        }

        switch (HelperService.pathModeSetting)
        {
            case PathModeSetting.EntireRelativePath: sb.Append(" -useEntireRelativePath"); break;
            case PathModeSetting.SelectedRelativePath: sb.Append(" -useSelectedRelativePath"); break;
        }

        foreach (dynamic path in HelperService.importedPaths)
        {
            sb.Append($" \"{LongDirectory.GetCleanPath(path.fullPath)}\"");
        }
        
        return sb.ToString();
    }

    /********************************************************************************************************************/

    internal static void ParseArgs(string[] args)
    {
        // Loading default values for settings
        buttonSetting = 0;
        selectionSetting = 0;
        cryptionSetting = CryptionOptions.Decrypt;

        bool isEncrypt = (args.Count(arg => !arg.StartsWith("-") && !arg.ToLower().EndsWith(".nhc")) > 0) || (args.FirstOrDefault(arg => arg.ToLower() == "-encrypt") != null);

        var argFiles = new List<Tuple<string, long>>();

        // Reading command line arguments and updating settings
        foreach (string arg in args)
        {
            var key = arg.ToLower();
            switch (key)
            {
                case "-useiconsonly": buttonSetting = ButtonSetting.IconsOnly; break;
                case "-usetextonly": buttonSetting = ButtonSetting.TextOnly; break;
                case "-useiconsandtext": buttonSetting = ButtonSetting.IconsAndText; break;
                case "-usecheckboxes": selectionSetting = SelectionSetting.Checkboxes; break;
                case "-usemultipleselection": selectionSetting = SelectionSetting.Multiselect; break;
                case "-windowcolorlightsteelblue": windowColorSetting = WindowColorSetting.LightSteelBlue; break;
                case "-windowcolordarkgray": windowColorSetting = WindowColorSetting.DarkGray; break;
                case "-windowcolordarkgrey": windowColorSetting = WindowColorSetting.DarkGray; break;
                case "-windowcolorwhite": windowColorSetting = WindowColorSetting.White; break;
                case "-useentirerelativepath": pathModeSetting = PathModeSetting.EntireRelativePath; break;
                case "-useselectedrelativepath": pathModeSetting = PathModeSetting.SelectedRelativePath; break;
                case "-forcemax": forceMAX = true; break;
                case "-forcelzma": forceLZMA = true; break;
                case "-usehighcompression": compressionLevel = CompressionLevel.Optimal; break;
                case "-usefastcompression": compressionLevel = CompressionLevel.Fastest; break;
                case "-usenocompression": compressionLevel = CompressionLevel.NoCompression; break;
                case "-useneedlecryptkey": cryptionAlgorithm = CryptionAlgorithm.NeedleCryptKey; break;
                case "-useneedlecryptpwd": cryptionAlgorithm = CryptionAlgorithm.NeedleCryptPassword; break;
                case "-userc2key": cryptionAlgorithm = CryptionAlgorithm.RC2Key; break;
                case "-userc2pwd": cryptionAlgorithm = CryptionAlgorithm.RC2Password; break;
                case "-usetripledeskey": cryptionAlgorithm = CryptionAlgorithm.TripleDesKey; break;
                case "-usetripledespwd": cryptionAlgorithm = CryptionAlgorithm.TripleDesPassword; break;
                case "-useaeskey": cryptionAlgorithm = CryptionAlgorithm.AesKey; break;
                case "-useaespwd": cryptionAlgorithm = CryptionAlgorithm.AesPassword; break;
                case "-setfileassociation": CreateFileAssociation(true); break;
                default:
                    if (key.StartsWith("-keyfilepath:"))
                        keyFileName = arg.Substring("-keyfilepath:".Length);
                    if (key.StartsWith("-password:"))
                        pwd = arg.Substring("-password:".Length);
                    if (key.StartsWith("-output:"))
                        output = arg.Substring("-output:".Length);
                    if (!key.StartsWith("-keyfilepath:") && !key.StartsWith("-password:") && !key.StartsWith("-output:"))
                    {
                        if (LongFile.Exists(arg))
                        {
                            long uncompressedFileSize = 0;
                            if (isEncrypt)
                            {
                                var stream = LongFile.GetFileStream(arg);
                                uncompressedFileSize = stream.Length;
                                stream.Close();
                                stream.Dispose();
                            }
                            argFiles.Add(new Tuple<string, long>(arg, uncompressedFileSize));
                        }
                    }
                    break;
            }
        }

        foreach (var file in argFiles)
        {
            if (isEncrypt) argPaths.Add(new { relativePath = Path.GetFileName(file.Item1), fullPath = LongFile.GetWin32LongPath(file.Item1), isDirectory = false, uncompressedFileSize = file.Item2 });
            else argPaths.Add(new { relativePath = Path.GetFileName(file.Item1), fullPath = LongFile.GetWin32LongPath(file.Item1), isDirectory = false, keyFilePath = keyFileName, password = pwd, cryptionAlgorithm });
        }

        if (argPaths.Count == 0)
        {
            cryptionSetting = CryptionOptions.Encrypt;
        }
        else cryptionSetting = isEncrypt ? CryptionOptions.Encrypt : CryptionOptions.Decrypt;
    }

    /********************************************************************************************************************/

    internal static void AppInitialization1()
    {
        if (argPaths != null && argPaths.Count > 0)
        {
            importedPaths = new ArrayList(argPaths);

            if (cryptionSetting == CryptionOptions.Decrypt)
            {
                foreach (dynamic importedPath in importedPaths)
                {
                    OpenArchiveForm openArchiveForm = new OpenArchiveForm(importedPath, updatedImportedPaths);
                    openArchiveForm.ShowDialog();
                    updatedImportedPaths = openArchiveForm.updatedImportedPaths;
                }

                importedPaths.Clear();
                importedPaths.AddRange(updatedImportedPaths);

                if (importedPaths.Count > 0)
                {
                    argPaths.Clear();
                    ProcessingForm frmProcessing = new ProcessingForm(ProcessingTask.OpenArchive, importedPaths);
                    frmProcessing.ShowDialog();
                }
            }
        }
    }

    /********************************************************************************************************************/

    internal static void AppInitialization2()
    {
        if (cryptionSetting == CryptionOptions.Decrypt)
        {
            updatedImportedPaths.Clear();

            foreach (dynamic importedPath in importedPaths)
            {
                if (importedPath.alreadyOpened)
                {
                    updatedImportedPaths.Add(importedPath);
                    continue;
                }
                OpenArchiveForm openArchiveForm = new OpenArchiveForm(importedPath, updatedImportedPaths);
                openArchiveForm.ShowDialog();
                updatedImportedPaths = openArchiveForm.updatedImportedPaths;
            }

            importedPaths.Clear();
            importedPaths.AddRange(updatedImportedPaths);

            if (importedPaths.Count > 0)
            {
                ProcessingForm frmProcessing = new ProcessingForm(ProcessingTask.OpenArchive, importedPaths);
                frmProcessing.ShowDialog();
            }
        }
    }

    /********************************************************************************************************************/

    private static Bitmap ResizeImage(Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }

    /********************************************************************************************************************/

    internal static Bitmap ResizeImagePercent(Image image, int percent)
    {
        if (image == null) return null;
        return ResizeImage(image, image.Width * percent / 100, image.Height * percent / 100);
    }
}

/********************************************************************************************************************/
