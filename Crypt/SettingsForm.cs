/********************************************************************************************************************
/ Needle in a Haystack in a Crypt v1.0.
/ Copyright (C) 2016-2023 by Horia Nedelciuc from Chisinau, Moldova.
/********************************************************************************************************************
/ Settings window.
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
    internal partial class SettingsForm : Form
    {
        internal SettingsForm()
        {
            InitializeComponent();
            SetWindowColor();

            comboBoxSelectionSetting.Items.Add(new Item("Checkboxes", 0));
            comboBoxSelectionSetting.Items.Add(new Item("Multiple Selection (CTRL / SHIFT + Click)", 1));

            comboBoxButtonSetting.Items.Add(new Item("Icons", 0));
            comboBoxButtonSetting.Items.Add(new Item("Text", 1));
            comboBoxButtonSetting.Items.Add(new Item("Icons + Text", 2));

            comboBoxWindowColorSetting.Items.Add(new Item("Light Steel Blue", 0));
            comboBoxWindowColorSetting.Items.Add(new Item("Dark Gray", 1));
            comboBoxWindowColorSetting.Items.Add(new Item("White", 2));

            comboBoxHighCompressionMode.Items.Add(new Item("Optimal (decent speed)", 0));
            comboBoxHighCompressionMode.Items.Add(new Item("Force LZMA Compression (slower)", 1));
            comboBoxHighCompressionMode.Items.Add(new Item("Force MAX Compression (slowest)", 2));

            comboBoxPathModeSetting.Items.Add(new Item("Entire Relative Path", 0));
            comboBoxPathModeSetting.Items.Add(new Item("Selected Relative Path", 1));

            initialSelectionSetting = HelperService.selectionSetting;
            initialButtonSetting = HelperService.buttonSetting;
            initialWindowColorSetting = HelperService.windowColorSetting;
            initialHighCompressionModeSetting = HelperService.forceMAX ? 2 : HelperService.forceLZMA ? 1 : 0;
            initialPathModeSetting = HelperService.pathModeSetting;
            
            comboBoxSelectionSetting.SelectedIndex = (int)HelperService.selectionSetting;
            comboBoxButtonSetting.SelectedIndex = (int)HelperService.buttonSetting;
            comboBoxWindowColorSetting.SelectedIndex = (int)HelperService.windowColorSetting;
            comboBoxHighCompressionMode.SelectedIndex = HelperService.forceMAX ? 2 : HelperService.forceLZMA ? 1 : 0;
            comboBoxPathModeSetting.SelectedIndex = (int)HelperService.pathModeSetting;

            // TO DO: Implement Path Mode
            comboBoxPathModeSetting.Visible = false;
            label4.Visible = false;
        }

        private HelperService.SelectionSetting initialSelectionSetting { get; set; }
        private HelperService.ButtonSetting initialButtonSetting { get; set; }
        private HelperService.WindowColorSetting initialWindowColorSetting { get; set; }
        private int initialHighCompressionModeSetting { get; set; }
        private HelperService.PathModeSetting initialPathModeSetting { get; set; }

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

        private void comboBoxSelectionSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item item = (Item)comboBoxSelectionSetting.SelectedItem;
            HelperService.selectionSetting = (HelperService.SelectionSetting)item.Value;
        }

        private void comboBoxButtonSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item item = (Item)comboBoxButtonSetting.SelectedItem;
            HelperService.buttonSetting = (HelperService.ButtonSetting)item.Value;
        }

        private void comboBoxWindowColorSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item item = (Item)comboBoxWindowColorSetting.SelectedItem;
            HelperService.windowColorSetting = (HelperService.WindowColorSetting)item.Value;
            SetWindowColor();
        }

        private void comboBoxHighCompressionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            HelperService.forceMAX = comboBoxHighCompressionMode.SelectedIndex == 2;
            HelperService.forceLZMA = comboBoxHighCompressionMode.SelectedIndex == 1;
        }

        private void comboBoxPathModeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display the Value property
            Item item = (Item)comboBoxPathModeSetting.SelectedItem;
            HelperService.pathModeSetting = (HelperService.PathModeSetting)item.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HelperService.selectionSetting = initialSelectionSetting;
            HelperService.buttonSetting = initialButtonSetting;
            HelperService.windowColorSetting = initialWindowColorSetting;
            HelperService.forceMAX = initialHighCompressionModeSetting == 2;
            HelperService.forceLZMA = initialHighCompressionModeSetting == 1;
            HelperService.pathModeSetting = initialPathModeSetting;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string cmdLineHelp = @"Command Line Help:
nhc.exe [-commandName1] | [-commandName2] | ... | [-commandNameN] | [""filePath1""] | [""filePath2""] | ... | [""filePathN""]
       -useIconsOnly
       -useTextOnly
       -useIconsAndText
       -useMultipleSelection
       -useCheckboxes
       -useEntireRelativePath
       -useSelectedRelativePath
       -windowColorLightSteelBlue
       -windowColorDarkGray
       -windowColorWhite
       -setFileAssociation
       -forceLZMA
       -useHighCompression
       -useFastCompression
       -useNoCompression
       -useNeedleCryptKey
       -useNeedleCryptPwd
       -useRC2Key
       -useRC2Pwd
       -useTripleDESKey
       -useTripleDESPwd
       -useAESKey
       -useAESPwd
       -encrypt
       -decrypt
       -keyFilePath:""yourKeyFilePath""
       -password:""yourPassword""";
            MessageBox.Show(cmdLineHelp, "CmdLine Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
