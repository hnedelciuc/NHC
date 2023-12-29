/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Open archive window.
/********************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypt
{
    internal partial class OpenArchiveForm : Form
    {
        dynamic importedPath;
        internal ArrayList updatedImportedPaths;
        internal bool error = false;

        /********************************************************************************************************************/

        internal OpenArchiveForm(dynamic importedPath, ArrayList updatedImportedPaths)
        {
            InitializeComponent();

            this.importedPath = importedPath;
            this.updatedImportedPaths = updatedImportedPaths;

            HelperService.scaling = GetDisplayScaleFactor();
            AdjustDisplayScaling();
            SetWindowColor();
            
            try
            {
                DetectArchiveService.DetectArchive(importedPath.fullPath);
            }
            catch (Exception ex)
            { 
                this.error = true;
                MessageBox.Show($"Could not open archive. {ex.Message}", "Error", MessageBoxButtons.OK);
                return;
            }
            
            toolTip.SetToolTip(btnBrowseKeyFile, "Browse Key");
            toolTip.SetToolTip(btnShowPassword, "Show");
            maskedTxtBox.UseSystemPasswordChar = true;
            lblAlgorithmName.Text = HelperService.cryptionAlgorithmDict[HelperService.cryptionAlgorithm].Replace("&", "&&");
            txtBoxKeyFile.Text = HelperService.IsKeyBasedCryptionAlgorithm() ? importedPath.keyFilePath : string.Empty;
            maskedTxtBox.Text = HelperService.IsPasswordBasedCryptionAlgorithm() ? importedPath.password : string.Empty;
            Text = Text + " - " + System.IO.Path.GetFileName(importedPath.fullPath);

            switch (HelperService.cryptionAlgorithm)
            {
                case HelperService.CryptionAlgorithm.NeedleCryptKey:
                    txtBoxKeyFile.Enabled = true;
                    txtBoxKeyFile.Select();
                    txtBoxKeyFile.Select(0, txtBoxKeyFile.Text.Length);
                    btnBrowseKeyFile.Enabled = true;
                    maskedTxtBox.Enabled = false;
                    btnShowPassword.Enabled = false;
                    break;
                case HelperService.CryptionAlgorithm.NeedleCryptPassword:
                    txtBoxKeyFile.Enabled = true;
                    btnBrowseKeyFile.Enabled = true;
                    maskedTxtBox.Enabled = true;
                    maskedTxtBox.Select();
                    maskedTxtBox.Select(0, maskedTxtBox.Text.Length);
                    btnShowPassword.Enabled = true;
                    break;
                case HelperService.CryptionAlgorithm.RC2Key:
                    txtBoxKeyFile.Enabled = true;
                    txtBoxKeyFile.Select();
                    txtBoxKeyFile.Select(0, txtBoxKeyFile.Text.Length);
                    btnBrowseKeyFile.Enabled = true;
                    maskedTxtBox.Enabled = false;
                    btnShowPassword.Enabled = false;
                    break;
                case HelperService.CryptionAlgorithm.RC2Password:
                    HelperService.keyFileName = null;
                    txtBoxKeyFile.Text = null;
                    txtBoxKeyFile.Enabled = false;
                    btnBrowseKeyFile.Enabled = false;
                    maskedTxtBox.Enabled = true;
                    maskedTxtBox.Select();
                    maskedTxtBox.Select(0, maskedTxtBox.Text.Length);
                    btnShowPassword.Enabled = true;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesKey:
                    txtBoxKeyFile.Enabled = true;
                    txtBoxKeyFile.Select();
                    txtBoxKeyFile.Select(0, txtBoxKeyFile.Text.Length);
                    btnBrowseKeyFile.Enabled = true;
                    maskedTxtBox.Enabled = false;
                    btnShowPassword.Enabled = false;
                    break;
                case HelperService.CryptionAlgorithm.TripleDesPassword:
                    HelperService.keyFileName = null;
                    txtBoxKeyFile.Text = null;
                    txtBoxKeyFile.Enabled = false;
                    btnBrowseKeyFile.Enabled = false;
                    maskedTxtBox.Enabled = true;
                    maskedTxtBox.Select();
                    maskedTxtBox.Select(0, maskedTxtBox.Text.Length);
                    btnShowPassword.Enabled = true;
                    break;
                case HelperService.CryptionAlgorithm.AesKey:
                    txtBoxKeyFile.Enabled = true;
                    txtBoxKeyFile.Select();
                    txtBoxKeyFile.Select(0, txtBoxKeyFile.Text.Length);
                    btnBrowseKeyFile.Enabled = true;
                    maskedTxtBox.Enabled = false;
                    btnShowPassword.Enabled = false;
                    break;
                case HelperService.CryptionAlgorithm.AesPassword:
                    HelperService.keyFileName = null;
                    txtBoxKeyFile.Text = null;
                    txtBoxKeyFile.Enabled = false;
                    btnBrowseKeyFile.Enabled = false;
                    maskedTxtBox.Enabled = true;
                    maskedTxtBox.Select();
                    maskedTxtBox.Select(0, maskedTxtBox.Text.Length);
                    btnShowPassword.Enabled = true;
                    break;
                default: return;
            }
        }

        /********************************************************************************************************************/

        OpenFileDialog openFileDialog = new OpenFileDialog();
        ToolTip toolTip = new ToolTip();
        bool isDiscard = true;

        /********************************************************************************************************************/

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string password;
            switch (HelperService.cryptionAlgorithm) 
            {
                case HelperService.CryptionAlgorithm.NeedleCryptKey:
                case HelperService.CryptionAlgorithm.RC2Key:
                case HelperService.CryptionAlgorithm.TripleDesKey:
                case HelperService.CryptionAlgorithm.AesKey:
                    password = null;
                    break;
                default: password = HelperService.pwd;
                    break;
            }
            this.updatedImportedPaths.Add(new { alreadyOpened = true, importedPath.fullPath, importedPath.relativePath, keyFilePath = HelperService.keyFileName, password, HelperService.cryptionAlgorithm });
            isDiscard = false;
            Close();
        }

        /********************************************************************************************************************/

        private void btnBrowseKeyFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Cryption Key File|*.nhk|Other File|*.*";
            openFileDialog.Title = "Choose Cryption Key Input File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                HelperService.keyFileName = openFileDialog.FileName;
                txtBoxKeyFile.Text = HelperService.keyFileName;
            }
        }

        /********************************************************************************************************************/

        private void txtBoxKeyFile_TextChanged(object sender, EventArgs e)
        {
            openFileDialog.FileName = txtBoxKeyFile.Text;
            HelperService.keyFileName = txtBoxKeyFile.Text;
        }

        /********************************************************************************************************************/

        private void txtBoxKeyFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /********************************************************************************************************************/

        private void txtBoxKeyFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (paths != null && paths.Length != 0)
            {
                if (!LongDirectory.Exists(paths[0]))
                {
                    txtBoxKeyFile.Text = paths[0];
                    openFileDialog.FileName = paths[0];
                    HelperService.keyFileName = paths[0];
                }
            }
        }

        /********************************************************************************************************************/
        private static bool showPassword = false;

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            showPassword = !showPassword;
            if (showPassword)
            {
                if (HelperService.buttonSetting == 0)
                    toolTip.SetToolTip(btnShowPassword, "Hide");
                else btnShowPassword.Text = "Hide";
                maskedTxtBox.UseSystemPasswordChar = false;
            }
            else
            {
                if (HelperService.buttonSetting == 0)
                    toolTip.SetToolTip(btnShowPassword, "Show");
                else btnShowPassword.Text = "Show";
                maskedTxtBox.UseSystemPasswordChar = true;
            }
        }

        /********************************************************************************************************************/

        private void maskedTxtBox_TextChanged(object sender, EventArgs e)
        {
            HelperService.pwd = maskedTxtBox.Text;
        }

        private void maskedTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOpen_Click(sender, e);
            }
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

        /********************************************************************************************************************/

        internal int GetDisplayScaleFactor()
        {
            float dpiX;
            Graphics graphics = CreateGraphics();
            dpiX = graphics.DpiX;
            return (int)(dpiX * 100 / 96);
        }

        /********************************************************************************************************************/

        private void AdjustDisplayScaling()
        {
            btnBrowseKeyFile.Image = HelperService.ResizeImagePercent(btnBrowseKeyFile.Image, HelperService.scaling);
            btnShowPassword.Image = HelperService.ResizeImagePercent(btnShowPassword.Image, HelperService.scaling);
        }

        /********************************************************************************************************************/

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isDiscard && HelperService.argPaths.Count > 0)
            {
                int i;
                for (i = 0; i < HelperService.argPaths.Count; i++)
                {
                    if (((dynamic)HelperService.argPaths[i]).fullPath == importedPath.fullPath)
                    {
                        break;
                    }
                }
                HelperService.argPaths.RemoveAt(i);
            }

            base.OnFormClosing(e);
        }

    }
}
