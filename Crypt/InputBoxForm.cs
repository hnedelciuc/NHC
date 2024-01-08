/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v0.9.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Input box window.
/********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypt
{
    internal partial class InputBoxForm : Form
    {
        internal int Result = 0;

        /********************************************************************************************************************/

        public InputBoxForm(string windowTitle, string labelText)
        {
            InitializeComponent();
            AdjustDisplayScaling();
            SetWindowColor();

            label.Text = labelText;
            Text = windowTitle;
            btnYesToAll.Visible = false;
            btnNoToAll.Visible = false;
        }

        /********************************************************************************************************************/

        public InputBoxForm(string windowTitle, string labelText, HelperService.ConfirmationButtons confirmationButtons)
        {
            InitializeComponent();
            SetWindowColor();

            label.Text = labelText;
            Text = windowTitle;
            
            if (confirmationButtons == HelperService.ConfirmationButtons.ShortcutGeneratorButtons)
            {
                btnBrowse.Visible = false;
                textBoxOutputFile.Visible = false;
                btnOk.Text = "Create Shortcut";
                btnNoToAll.Text = "Create File Association";
                btnYesToAll.Text = "Create 'Send To...' Shortcut";
                ComponentResourceManager resources = new ComponentResourceManager(typeof(CryptForm));
                label.Image = (Image)resources.GetObject("btnShortcut.Image");
                label.ImageAlign = ContentAlignment.MiddleCenter;
            }
            else
            {
                btnBrowse.Visible = false;
                textBoxOutputFile.Visible = false;
                btnOk.Text = "Yes";
                btnCancel.Text = "No";
            }

            AdjustDisplayScaling();
        }

        internal string Value { get; set; }
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        /********************************************************************************************************************/

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = 0;
            Close();
        }

        /********************************************************************************************************************/

        private void btnOk_Click(object sender, EventArgs e)
        {
            Value = textBoxOutputFile.Text;
            Result = 1;
            Close();
        }

        /********************************************************************************************************************/

        private void btnNoToAll_Click(object sender, EventArgs e)
        {
            Result = 2;
            Close();
        }

        /********************************************************************************************************************/

        private void btnYesToAll_Click(object sender, EventArgs e)
        {
            Result = 3;
            Close();
        }

        /********************************************************************************************************************/

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Crypt File|*.nhc|Other File|*.*";
            saveFileDialog.Title = "Choose Output File (archive)";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxOutputFile.Text = saveFileDialog.FileName;
            }
        }

        /********************************************************************************************************************/

        private void textBoxOutputFile_Leave(object sender, System.EventArgs e)
        {
            if (!LongFile.Exists(textBoxOutputFile.Text)
            || (MessageBox.Show("Output file " + textBoxOutputFile.Text + " already exists. Do you want to replace it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
            {
                saveFileDialog.FileName = textBoxOutputFile.Text;
            }
            else textBoxOutputFile.Text = String.Empty;
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
                if (!LongDirectory.Exists(paths[0]))
                {
                    if (!LongFile.Exists(paths[0])
                    || (MessageBox.Show("Output file " + paths[0] + " already exists. Do you want to replace it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
                    {
                        textBoxOutputFile.Text = paths[0];
                        saveFileDialog.FileName = paths[0];
                    }
                    else textBoxOutputFile.Text = String.Empty;
                }
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

        private void AdjustDisplayScaling()
        {
            btnBrowse.Image = HelperService.ResizeImagePercent(btnBrowse.Image, HelperService.scaling);
            if (label.Image != null)
            {
                label.Image = HelperService.ResizeImagePercent(label.Image, HelperService.scaling + HelperService.scaling / 2);
            }
        }
    }
}
