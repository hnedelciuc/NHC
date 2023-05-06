using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;


namespace Crypt
{
    /* Class  : FileExplorer
     * Author : Chandana Subasinghe
     * Date   : 10/03/2006
     * Discription : This class is used to create the tree view and load 
     *               directories and files into the tree
     *          
     */
    class FileFolderExplorer
    {
        internal FileFolderExplorer()
        {

        }

        /* Method : CreateTree
         * Author : Chandana Subasinghe
         * Date   : 10/03/2006
         * Discription : This is used to create and build the tree
         *          
         */

        internal bool CreateTree(TreeViewMS treeView)
        {
            bool returnValue = false;
                 
            try
            {
                // Create Desktop
                TreeNode desktop = new TreeNode() { ImageIndex = 2, SelectedImageIndex = 2 };
                desktop.Text = "Desktop";
                desktop.Name = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\";
                desktop.Nodes.Add("");
                desktop.Tag = new { fullPath = desktop.Name, relativePath = LongDirectory.GetDirectoryName(desktop.Name) };
                treeView.Nodes.Add(desktop);
                // Create UserProfile
                TreeNode myUserProfile = new TreeNode() { ImageIndex = 2, SelectedImageIndex = 2 };
                myUserProfile.Name = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";
                myUserProfile.Text = LongDirectory.GetJustDirectoryName(myUserProfile.Name.TrimEnd('\\'));
                myUserProfile.Tag = new { fullPath = myUserProfile.Name, relativePath = LongDirectory.GetDirectoryName(myUserProfile.Name) };
                myUserProfile.Nodes.Add("");
                treeView.Nodes.Add(myUserProfile);
                // Create Downloads
                TreeNode myDownloads = new TreeNode() { ImageIndex = 2, SelectedImageIndex = 2 };
                myDownloads.Text = "Downloads";
                myDownloads.Name = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";
                myDownloads.Tag = new { fullPath = myDownloads.Name, relativePath = LongDirectory.GetDirectoryName(myDownloads.Name) };
                myDownloads.Nodes.Add("");
                treeView.Nodes.Add(myDownloads);
                // Create Documents
                TreeNode myDocuments = new TreeNode() { ImageIndex = 7, SelectedImageIndex = 7 };
                myDocuments.Text = "Documents";
                myDocuments.Name = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
                myDocuments.Tag = new { fullPath = myDocuments.Name, relativePath = LongDirectory.GetDirectoryName(myDocuments.Name) };
                myDocuments.Nodes.Add("");
                treeView.Nodes.Add(myDocuments);
                // Create Pictures
                TreeNode myPictures = new TreeNode() { ImageIndex = 8, SelectedImageIndex = 8 };
                myPictures.Text = "Pictures";
                myPictures.Name = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\";
                myPictures.Tag = new { fullPath = myPictures.Name, relativePath = LongDirectory.GetDirectoryName(myPictures.Name) };
                myPictures.Nodes.Add("");
                treeView.Nodes.Add(myPictures);
                // Create Music
                TreeNode myMusic = new TreeNode() { ImageIndex = 10, SelectedImageIndex = 10 };
                myMusic.Text = "Music";
                myMusic.Name = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";
                myMusic.Tag = new { fullPath = myMusic.Name, relativePath = LongDirectory.GetDirectoryName(myMusic.Name) };
                myMusic.Nodes.Add("");
                treeView.Nodes.Add(myMusic);
                // Create Videos
                TreeNode myVideos = new TreeNode() { ImageIndex = 9, SelectedImageIndex = 9 };
                myVideos.Text = "Videos";
                myVideos.Name = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\";
                myVideos.Tag = new { fullPath = myVideos.Name, relativePath = LongDirectory.GetDirectoryName(myVideos.Name) };
                myVideos.Nodes.Add("");
                treeView.Nodes.Add(myVideos);
                // Create Network
                TreeNode myNetwork = new TreeNode() { ImageIndex = 14, SelectedImageIndex = 14 };
                myNetwork.Text = "Network";
                myNetwork.Name = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts) + "\\";
                myNetwork.Tag = new { fullPath = myNetwork.Name, relativePath = LongDirectory.GetDirectoryName(myNetwork.Name) };
                myNetwork.Nodes.Add("");
                treeView.Nodes.Add(myNetwork);
                // Create Drives
                foreach (DriveInfo drv in DriveInfo.GetDrives())
                {
                    
                    TreeNode fChild = new TreeNode();
                    if (drv.DriveType == DriveType.CDRom)
                    {
                        fChild.ImageIndex = 13;
                        fChild.SelectedImageIndex = 13;
                    }
                    else if (drv.DriveType == DriveType.Fixed)
                    {
                        fChild.ImageIndex = 12;
                        fChild.SelectedImageIndex = 12;
                    }
                    fChild.Text = drv.Name;
                    fChild.Name = drv.Name;
                    fChild.Tag = new { fullPath = drv.Name, relativePath = "Drive" + drv.Name.Replace(":\\", "") };
                    fChild.Nodes.Add("");
                    treeView.Nodes.Add(fChild);
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                returnValue = false;
            }
            return returnValue;
            
        }

        /* Method : EnumerateDirectory
         * Author : Chandana Subasinghe
         * Date   : 10/03/2006
         * Discription : This is used to Enumerate directories and files
         *          
         */
        internal TreeNode EnumerateDirectory(TreeNode parentNode)
        {
          
            try
            {
                DirectoryInfo rootDir;

                // To fill Desktop
                Char [] arr={'\\'};
                string [] nameList=parentNode.FullPath.Split(arr);
                string path = "";

                switch (nameList.GetValue(0).ToString())
                {
                    case "Desktop": 
                        path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Downloads":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Documents":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Pictures":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Music":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Videos":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    case "Network":
                        path = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts) + "\\";

                        for (int i = 1; i < nameList.Length; i++)
                        {
                            path = path + nameList[i] + "\\";
                        }

                        rootDir = new DirectoryInfo(path);
                        break;
                    default:
                        // for User Profile
                        if (!nameList.GetValue(0).ToString().Contains(":"))
                        {
                            path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";

                            for (int i = 1; i < nameList.Length; i++)
                            {
                                path = path + nameList[i] + "\\";
                            }

                            rootDir = new DirectoryInfo(path);
                        }
                        else // for other Directories
                            rootDir = new DirectoryInfo(parentNode.FullPath + "\\");
                        break;
                }

                parentNode.Nodes[0].Remove();
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    
                    TreeNode node = new TreeNode() { ImageIndex = 2, SelectedImageIndex = 2 };
                    node.Text = dir.Name;
                    node.Name = rootDir + dir.Name;
                    node.Nodes.Add("");
                    parentNode.Nodes.Add(node);
                }
                //Fill files
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    TreeNode node = new TreeNode();
                    node.Text = file.Name;
                    node.Name = rootDir + file.Name;
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    parentNode.Nodes.Add(node);
                }



            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
            return parentNode;
        }
    }
}
