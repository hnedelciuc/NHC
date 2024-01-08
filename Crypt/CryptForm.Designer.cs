namespace Crypt
{
    partial class CryptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CryptForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.textBoxOutputFile = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxKeyFile = new System.Windows.Forms.TextBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxChooseCryptionKey = new System.Windows.Forms.GroupBox();
            this.radioButtonGenerateNewKey = new System.Windows.Forms.RadioButton();
            this.radioButtonUseExistingKey = new System.Windows.Forms.RadioButton();
            this.radioButtonUseBuiltInDefault = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.maskedTextBoxPassword = new System.Windows.Forms.MaskedTextBox();
            this.btnBrowseKey = new System.Windows.Forms.Button();
            this.btnShowHidePassword = new System.Windows.Forms.Button();
            this.groupBoxChooseCryptionAlgorithm = new System.Windows.Forms.GroupBox();
            this.comboBoxChooseCryptionAlgorithm = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.checkBoxIsSplitArchive = new System.Windows.Forms.CheckBox();
            this.comboBoxSplitArchiveSize = new System.Windows.Forms.ComboBox();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnShortcut = new System.Windows.Forms.Button();
            this.groupBoxChooseCompressionLevel = new System.Windows.Forms.GroupBox();
            this.radioButtonCompressionNone = new System.Windows.Forms.RadioButton();
            this.radioButtonCompressionLow = new System.Windows.Forms.RadioButton();
            this.radioButtonCompressionHigh = new System.Windows.Forms.RadioButton();
            this.groupBoxOverwriteFiles = new System.Windows.Forms.GroupBox();
            this.radioButtonOverwriteFilesAsk = new System.Windows.Forms.RadioButton();
            this.radioButtonOverwriteFilesNo = new System.Windows.Forms.RadioButton();
            this.radioButtonOverwriteFilesYes = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView1 = new Crypt.TreeViewMS();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxChooseOperatingMode = new System.Windows.Forms.GroupBox();
            this.radioButtonDecompressDecrypt = new System.Windows.Forms.RadioButton();
            this.radioButtonCompressEncrypt = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBrowseInput = new System.Windows.Forms.Button();
            this.btnUpdateArchive1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCompressEncrypt = new System.Windows.Forms.Button();
            this.btnCompressEncryptExtra = new System.Windows.Forms.Button();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCheckKey = new System.Windows.Forms.Button();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGenerateKey = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxChooseCryptionKey.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxChooseCryptionAlgorithm.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxChooseCompressionLevel.SuspendLayout();
            this.groupBoxOverwriteFiles.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBoxChooseOperatingMode.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // textBoxOutputFile
            // 
            this.textBoxOutputFile.AllowDrop = true;
            this.textBoxOutputFile.Location = new System.Drawing.Point(4, 20);
            this.textBoxOutputFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxOutputFile.Name = "textBoxOutputFile";
            this.textBoxOutputFile.Size = new System.Drawing.Size(338, 20);
            this.textBoxOutputFile.TabIndex = 0;
            this.textBoxOutputFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxOutputFile_DragDrop);
            this.textBoxOutputFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxOutputFile_DragEnter);
            this.textBoxOutputFile.Leave += new System.EventHandler(this.textBoxOutputFile_Leave);
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseOutput.Image = global::Crypt.Properties.Resources.Plus__3D__mini;
            this.btnBrowseOutput.Location = new System.Drawing.Point(352, 3);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.tableLayoutPanel1.SetRowSpan(this.btnBrowseOutput, 2);
            this.btnBrowseOutput.Size = new System.Drawing.Size(58, 50);
            this.btnBrowseOutput.TabIndex = 0;
            this.btnBrowseOutput.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnBrowseOutput, "Browse");
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please choose input files to be archived:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.Location = new System.Drawing.Point(3, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(315, 14);
            this.label2.TabIndex = 0;
            this.label2.Text = "Please choose output file (archive):";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(317, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Please choose existing encryption key file:";
            // 
            // textBoxKeyFile
            // 
            this.textBoxKeyFile.AllowDrop = true;
            this.textBoxKeyFile.Enabled = false;
            this.textBoxKeyFile.Location = new System.Drawing.Point(4, 77);
            this.textBoxKeyFile.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxKeyFile.Name = "textBoxKeyFile";
            this.textBoxKeyFile.Size = new System.Drawing.Size(338, 20);
            this.textBoxKeyFile.TabIndex = 0;
            this.textBoxKeyFile.TextChanged += new System.EventHandler(this.textBoxKeyFile_TextChanged);
            this.textBoxKeyFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBoxKeyFile_DragDrop);
            this.textBoxKeyFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBoxKeyFile_DragEnter);
            this.textBoxKeyFile.Leave += new System.EventHandler(this.textBoxKeyFile_Leave);
            // 
            // groupBoxChooseCryptionKey
            // 
            this.groupBoxChooseCryptionKey.Controls.Add(this.radioButtonGenerateNewKey);
            this.groupBoxChooseCryptionKey.Controls.Add(this.radioButtonUseExistingKey);
            this.groupBoxChooseCryptionKey.Controls.Add(this.radioButtonUseBuiltInDefault);
            this.groupBoxChooseCryptionKey.Location = new System.Drawing.Point(421, 379);
            this.groupBoxChooseCryptionKey.Name = "groupBoxChooseCryptionKey";
            this.groupBoxChooseCryptionKey.Size = new System.Drawing.Size(234, 93);
            this.groupBoxChooseCryptionKey.TabIndex = 0;
            this.groupBoxChooseCryptionKey.TabStop = false;
            this.groupBoxChooseCryptionKey.Text = "Encryption Key";
            // 
            // radioButtonGenerateNewKey
            // 
            this.radioButtonGenerateNewKey.AutoSize = true;
            this.radioButtonGenerateNewKey.Location = new System.Drawing.Point(12, 66);
            this.radioButtonGenerateNewKey.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonGenerateNewKey.Name = "radioButtonGenerateNewKey";
            this.radioButtonGenerateNewKey.Size = new System.Drawing.Size(168, 17);
            this.radioButtonGenerateNewKey.TabIndex = 0;
            this.radioButtonGenerateNewKey.Text = "Generate New Encryption Key";
            this.radioButtonGenerateNewKey.UseVisualStyleBackColor = true;
            this.radioButtonGenerateNewKey.CheckedChanged += new System.EventHandler(this.radioButtonGenerateNewKey_CheckedChanged);
            // 
            // radioButtonUseExistingKey
            // 
            this.radioButtonUseExistingKey.AutoSize = true;
            this.radioButtonUseExistingKey.Location = new System.Drawing.Point(12, 45);
            this.radioButtonUseExistingKey.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonUseExistingKey.Name = "radioButtonUseExistingKey";
            this.radioButtonUseExistingKey.Size = new System.Drawing.Size(157, 17);
            this.radioButtonUseExistingKey.TabIndex = 0;
            this.radioButtonUseExistingKey.Text = "Use Existing Encryption Key";
            this.radioButtonUseExistingKey.UseVisualStyleBackColor = true;
            this.radioButtonUseExistingKey.CheckedChanged += new System.EventHandler(this.radioButtonUseExistingKey_CheckedChanged);
            // 
            // radioButtonUseBuiltInDefault
            // 
            this.radioButtonUseBuiltInDefault.AutoSize = true;
            this.radioButtonUseBuiltInDefault.Checked = true;
            this.radioButtonUseBuiltInDefault.Location = new System.Drawing.Point(12, 23);
            this.radioButtonUseBuiltInDefault.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonUseBuiltInDefault.Name = "radioButtonUseBuiltInDefault";
            this.radioButtonUseBuiltInDefault.Size = new System.Drawing.Size(191, 17);
            this.radioButtonUseBuiltInDefault.TabIndex = 0;
            this.radioButtonUseBuiltInDefault.TabStop = true;
            this.radioButtonUseBuiltInDefault.Text = "No Encryption Key (Built-In Default)";
            this.radioButtonUseBuiltInDefault.UseVisualStyleBackColor = true;
            this.radioButtonUseBuiltInDefault.CheckedChanged += new System.EventHandler(this.radioButtonUseBuiltInDefault_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 349F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxOutputFile, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxKeyFile, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.maskedTextBoxPassword, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseKey, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseOutput, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnShowHidePassword, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 315);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(414, 170);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(3, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(317, 14);
            this.label5.TabIndex = 0;
            this.label5.Text = "Please type encryption password:";
            // 
            // maskedTextBoxPassword
            // 
            this.maskedTextBoxPassword.Enabled = false;
            this.maskedTextBoxPassword.Location = new System.Drawing.Point(4, 134);
            this.maskedTextBoxPassword.Margin = new System.Windows.Forms.Padding(4);
            this.maskedTextBoxPassword.Name = "maskedTextBoxPassword";
            this.maskedTextBoxPassword.Size = new System.Drawing.Size(338, 20);
            this.maskedTextBoxPassword.TabIndex = 0;
            this.maskedTextBoxPassword.UseSystemPasswordChar = true;
            this.maskedTextBoxPassword.TextChanged += new System.EventHandler(this.maskedTextBoxPassword_TextChanged);
            // 
            // btnBrowseKey
            // 
            this.btnBrowseKey.Enabled = false;
            this.btnBrowseKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseKey.Image = global::Crypt.Properties.Resources.Plus__3D__mini;
            this.btnBrowseKey.Location = new System.Drawing.Point(352, 60);
            this.btnBrowseKey.Name = "btnBrowseKey";
            this.tableLayoutPanel1.SetRowSpan(this.btnBrowseKey, 2);
            this.btnBrowseKey.Size = new System.Drawing.Size(58, 50);
            this.btnBrowseKey.TabIndex = 0;
            this.btnBrowseKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnBrowseKey, "Browse");
            this.btnBrowseKey.UseVisualStyleBackColor = true;
            this.btnBrowseKey.Click += new System.EventHandler(this.btnBrowseKey_Click);
            // 
            // btnShowHidePassword
            // 
            this.btnShowHidePassword.Enabled = false;
            this.btnShowHidePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowHidePassword.Image = global::Crypt.Properties.Resources.Eye__3D__mini;
            this.btnShowHidePassword.Location = new System.Drawing.Point(352, 117);
            this.btnShowHidePassword.Name = "btnShowHidePassword";
            this.tableLayoutPanel1.SetRowSpan(this.btnShowHidePassword, 2);
            this.btnShowHidePassword.Size = new System.Drawing.Size(58, 50);
            this.btnShowHidePassword.TabIndex = 0;
            this.btnShowHidePassword.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnShowHidePassword, "Show");
            this.btnShowHidePassword.UseVisualStyleBackColor = true;
            this.btnShowHidePassword.Click += new System.EventHandler(this.btnShowHidePassword_Click);
            // 
            // groupBoxChooseCryptionAlgorithm
            // 
            this.groupBoxChooseCryptionAlgorithm.Controls.Add(this.comboBoxChooseCryptionAlgorithm);
            this.groupBoxChooseCryptionAlgorithm.Location = new System.Drawing.Point(422, 314);
            this.groupBoxChooseCryptionAlgorithm.Name = "groupBoxChooseCryptionAlgorithm";
            this.groupBoxChooseCryptionAlgorithm.Size = new System.Drawing.Size(233, 55);
            this.groupBoxChooseCryptionAlgorithm.TabIndex = 1;
            this.groupBoxChooseCryptionAlgorithm.TabStop = false;
            this.groupBoxChooseCryptionAlgorithm.Text = "Encryption Algorithm";
            // 
            // comboBoxChooseCryptionAlgorithm
            // 
            this.comboBoxChooseCryptionAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChooseCryptionAlgorithm.FormattingEnabled = true;
            this.comboBoxChooseCryptionAlgorithm.Location = new System.Drawing.Point(11, 18);
            this.comboBoxChooseCryptionAlgorithm.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxChooseCryptionAlgorithm.Name = "comboBoxChooseCryptionAlgorithm";
            this.comboBoxChooseCryptionAlgorithm.Size = new System.Drawing.Size(199, 21);
            this.comboBoxChooseCryptionAlgorithm.TabIndex = 0;
            this.comboBoxChooseCryptionAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxChooseCryptionAlgorithm_SelectedIndexChanged);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.47107F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.61345F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.19328F));
            this.tableLayoutPanel3.Controls.Add(this.checkBoxIsSplitArchive, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.comboBoxSplitArchiveSize, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnAbout, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnSettings, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnShortcut, 2, 3);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(417, 317);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.34008F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.65992F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(238, 245);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // checkBoxIsSplitArchive
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.checkBoxIsSplitArchive, 2);
            this.checkBoxIsSplitArchive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.checkBoxIsSplitArchive.Location = new System.Drawing.Point(7, 161);
            this.checkBoxIsSplitArchive.Margin = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.checkBoxIsSplitArchive.Name = "checkBoxIsSplitArchive";
            this.checkBoxIsSplitArchive.Size = new System.Drawing.Size(149, 30);
            this.checkBoxIsSplitArchive.TabIndex = 1;
            this.checkBoxIsSplitArchive.Text = "Split Archive            Size:";
            this.checkBoxIsSplitArchive.UseVisualStyleBackColor = true;
            // 
            // comboBoxSplitArchiveSize
            // 
            this.comboBoxSplitArchiveSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSplitArchiveSize.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxSplitArchiveSize.CausesValidation = false;
            this.comboBoxSplitArchiveSize.FormattingEnabled = true;
            this.comboBoxSplitArchiveSize.Items.AddRange(new object[] {
            "2 GB",
            "1 GB",
            "500 MB",
            "100 MB",
            "50 MB",
            "10 MB",
            "1 MB",
            "500 KB"});
            this.comboBoxSplitArchiveSize.Location = new System.Drawing.Point(161, 165);
            this.comboBoxSplitArchiveSize.Name = "comboBoxSplitArchiveSize";
            this.comboBoxSplitArchiveSize.Size = new System.Drawing.Size(74, 21);
            this.comboBoxSplitArchiveSize.TabIndex = 2;
            this.comboBoxSplitArchiveSize.TextUpdate += new System.EventHandler(this.comboBoxSplitArchiveSize_TextUpdate);
            this.comboBoxSplitArchiveSize.DropDownClosed += new System.EventHandler(this.comboBoxSplitArchiveSize_DropDownClosed);
            this.comboBoxSplitArchiveSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxSplitArchiveSize_KeyUp);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.Location = new System.Drawing.Point(2, 196);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(2);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 47);
            this.btnAbout.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnAbout, "About NHC");
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Location = new System.Drawing.Point(81, 196);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 47);
            this.btnSettings.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnSettings, "Settings");
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnShortcut
            // 
            this.btnShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShortcut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShortcut.Location = new System.Drawing.Point(160, 196);
            this.btnShortcut.Margin = new System.Windows.Forms.Padding(2);
            this.btnShortcut.Name = "btnShortcut";
            this.btnShortcut.Size = new System.Drawing.Size(75, 47);
            this.btnShortcut.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnShortcut, "Shortcut");
            this.btnShortcut.UseVisualStyleBackColor = true;
            this.btnShortcut.Click += new System.EventHandler(this.btnShortcut_Click);
            // 
            // groupBoxChooseCompressionLevel
            // 
            this.groupBoxChooseCompressionLevel.Controls.Add(this.radioButtonCompressionNone);
            this.groupBoxChooseCompressionLevel.Controls.Add(this.radioButtonCompressionLow);
            this.groupBoxChooseCompressionLevel.Controls.Add(this.radioButtonCompressionHigh);
            this.groupBoxChooseCompressionLevel.Location = new System.Drawing.Point(2, 500);
            this.groupBoxChooseCompressionLevel.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxChooseCompressionLevel.Name = "groupBoxChooseCompressionLevel";
            this.groupBoxChooseCompressionLevel.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxChooseCompressionLevel.Size = new System.Drawing.Size(232, 50);
            this.groupBoxChooseCompressionLevel.TabIndex = 5;
            this.groupBoxChooseCompressionLevel.TabStop = false;
            this.groupBoxChooseCompressionLevel.Text = "Choose Compression Level";
            // 
            // radioButtonCompressionNone
            // 
            this.radioButtonCompressionNone.AutoSize = true;
            this.radioButtonCompressionNone.Location = new System.Drawing.Point(125, 24);
            this.radioButtonCompressionNone.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonCompressionNone.Name = "radioButtonCompressionNone";
            this.radioButtonCompressionNone.Size = new System.Drawing.Size(102, 17);
            this.radioButtonCompressionNone.TabIndex = 0;
            this.radioButtonCompressionNone.TabStop = true;
            this.radioButtonCompressionNone.Text = "No Compression";
            this.radioButtonCompressionNone.UseVisualStyleBackColor = true;
            this.radioButtonCompressionNone.CheckedChanged += new System.EventHandler(this.radioButtonCompressionNone_CheckedChanged);
            // 
            // radioButtonCompressionLow
            // 
            this.radioButtonCompressionLow.AutoSize = true;
            this.radioButtonCompressionLow.Checked = true;
            this.radioButtonCompressionLow.Location = new System.Drawing.Point(72, 24);
            this.radioButtonCompressionLow.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonCompressionLow.Name = "radioButtonCompressionLow";
            this.radioButtonCompressionLow.Size = new System.Drawing.Size(45, 17);
            this.radioButtonCompressionLow.TabIndex = 0;
            this.radioButtonCompressionLow.TabStop = true;
            this.radioButtonCompressionLow.Text = "Fast";
            this.radioButtonCompressionLow.UseVisualStyleBackColor = true;
            this.radioButtonCompressionLow.CheckedChanged += new System.EventHandler(this.radioButtonCompressionLow_CheckedChanged);
            // 
            // radioButtonCompressionHigh
            // 
            this.radioButtonCompressionHigh.AutoSize = true;
            this.radioButtonCompressionHigh.Location = new System.Drawing.Point(15, 24);
            this.radioButtonCompressionHigh.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonCompressionHigh.Name = "radioButtonCompressionHigh";
            this.radioButtonCompressionHigh.Size = new System.Drawing.Size(47, 17);
            this.radioButtonCompressionHigh.TabIndex = 0;
            this.radioButtonCompressionHigh.TabStop = true;
            this.radioButtonCompressionHigh.Text = "High";
            this.radioButtonCompressionHigh.UseVisualStyleBackColor = true;
            this.radioButtonCompressionHigh.CheckedChanged += new System.EventHandler(this.radioButtonCompressionHigh_CheckedChanged);
            // 
            // groupBoxOverwriteFiles
            // 
            this.groupBoxOverwriteFiles.Controls.Add(this.radioButtonOverwriteFilesAsk);
            this.groupBoxOverwriteFiles.Controls.Add(this.radioButtonOverwriteFilesNo);
            this.groupBoxOverwriteFiles.Controls.Add(this.radioButtonOverwriteFilesYes);
            this.groupBoxOverwriteFiles.Enabled = false;
            this.groupBoxOverwriteFiles.Location = new System.Drawing.Point(244, 500);
            this.groupBoxOverwriteFiles.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxOverwriteFiles.Name = "groupBoxOverwriteFiles";
            this.groupBoxOverwriteFiles.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxOverwriteFiles.Size = new System.Drawing.Size(168, 50);
            this.groupBoxOverwriteFiles.TabIndex = 3;
            this.groupBoxOverwriteFiles.TabStop = false;
            this.groupBoxOverwriteFiles.Text = "Overwrite Existing Files";
            // 
            // radioButtonOverwriteFilesAsk
            // 
            this.radioButtonOverwriteFilesAsk.AutoSize = true;
            this.radioButtonOverwriteFilesAsk.Checked = true;
            this.radioButtonOverwriteFilesAsk.Location = new System.Drawing.Point(113, 24);
            this.radioButtonOverwriteFilesAsk.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonOverwriteFilesAsk.Name = "radioButtonOverwriteFilesAsk";
            this.radioButtonOverwriteFilesAsk.Size = new System.Drawing.Size(43, 17);
            this.radioButtonOverwriteFilesAsk.TabIndex = 0;
            this.radioButtonOverwriteFilesAsk.TabStop = true;
            this.radioButtonOverwriteFilesAsk.Text = "Ask";
            this.radioButtonOverwriteFilesAsk.UseVisualStyleBackColor = true;
            // 
            // radioButtonOverwriteFilesNo
            // 
            this.radioButtonOverwriteFilesNo.AutoSize = true;
            this.radioButtonOverwriteFilesNo.Location = new System.Drawing.Point(68, 24);
            this.radioButtonOverwriteFilesNo.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonOverwriteFilesNo.Name = "radioButtonOverwriteFilesNo";
            this.radioButtonOverwriteFilesNo.Size = new System.Drawing.Size(39, 17);
            this.radioButtonOverwriteFilesNo.TabIndex = 0;
            this.radioButtonOverwriteFilesNo.Text = "No";
            this.radioButtonOverwriteFilesNo.UseVisualStyleBackColor = true;
            // 
            // radioButtonOverwriteFilesYes
            // 
            this.radioButtonOverwriteFilesYes.AutoSize = true;
            this.radioButtonOverwriteFilesYes.Location = new System.Drawing.Point(17, 24);
            this.radioButtonOverwriteFilesYes.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonOverwriteFilesYes.Name = "radioButtonOverwriteFilesYes";
            this.radioButtonOverwriteFilesYes.Size = new System.Drawing.Size(43, 17);
            this.radioButtonOverwriteFilesYes.TabIndex = 0;
            this.radioButtonOverwriteFilesYes.Text = "Yes";
            this.radioButtonOverwriteFilesYes.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel1.Controls.Add(this.groupBoxChooseCryptionKey);
            this.panel1.Controls.Add(this.groupBoxChooseCryptionAlgorithm);
            this.panel1.Controls.Add(this.treeView1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.groupBoxOverwriteFiles);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.groupBoxChooseCompressionLevel);
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Location = new System.Drawing.Point(9, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(660, 565);
            this.panel1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(8, 23);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.Size = new System.Drawing.Size(404, 284);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCollapse);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.16667F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.83333F));
            this.tableLayoutPanel2.Controls.Add(this.groupBoxChooseOperatingMode, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel8, 1, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(418, 6);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(240, 301);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // groupBoxChooseOperatingMode
            // 
            this.groupBoxChooseOperatingMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxChooseOperatingMode.Controls.Add(this.radioButtonDecompressDecrypt);
            this.groupBoxChooseOperatingMode.Controls.Add(this.radioButtonCompressEncrypt);
            this.groupBoxChooseOperatingMode.Location = new System.Drawing.Point(4, 0);
            this.groupBoxChooseOperatingMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 2);
            this.groupBoxChooseOperatingMode.Name = "groupBoxChooseOperatingMode";
            this.groupBoxChooseOperatingMode.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.groupBoxChooseOperatingMode.Size = new System.Drawing.Size(110, 97);
            this.groupBoxChooseOperatingMode.TabIndex = 0;
            this.groupBoxChooseOperatingMode.TabStop = false;
            this.groupBoxChooseOperatingMode.Text = "Choose";
            // 
            // radioButtonDecompressDecrypt
            // 
            this.radioButtonDecompressDecrypt.Location = new System.Drawing.Point(8, 52);
            this.radioButtonDecompressDecrypt.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonDecompressDecrypt.Name = "radioButtonDecompressDecrypt";
            this.radioButtonDecompressDecrypt.Size = new System.Drawing.Size(101, 35);
            this.radioButtonDecompressDecrypt.TabIndex = 0;
            this.radioButtonDecompressDecrypt.TabStop = true;
            this.radioButtonDecompressDecrypt.Text = "Extract Archive";
            this.radioButtonDecompressDecrypt.UseVisualStyleBackColor = true;
            this.radioButtonDecompressDecrypt.CheckedChanged += new System.EventHandler(this.radioButtonDecompressDecrypt_CheckedChanged);
            // 
            // radioButtonCompressEncrypt
            // 
            this.radioButtonCompressEncrypt.Checked = true;
            this.radioButtonCompressEncrypt.Location = new System.Drawing.Point(8, 23);
            this.radioButtonCompressEncrypt.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonCompressEncrypt.Name = "radioButtonCompressEncrypt";
            this.radioButtonCompressEncrypt.Size = new System.Drawing.Size(101, 32);
            this.radioButtonCompressEncrypt.TabIndex = 0;
            this.radioButtonCompressEncrypt.TabStop = true;
            this.radioButtonCompressEncrypt.Text = "Create Archive";
            this.radioButtonCompressEncrypt.UseVisualStyleBackColor = true;
            this.radioButtonCompressEncrypt.CheckedChanged += new System.EventHandler(this.radioButtonCompressEncrypt_CheckedChanged);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.66102F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.33898F));
            this.tableLayoutPanel4.Controls.Add(this.btnBrowseInput, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnUpdateArchive1, 1, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 101);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0, 1, 0, 4);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(118, 94);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // btnBrowseInput
            // 
            this.btnBrowseInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseInput.Image = global::Crypt.Properties.Resources.Plus__3D__small_2;
            this.btnBrowseInput.Location = new System.Drawing.Point(3, 3);
            this.btnBrowseInput.Name = "btnBrowseInput";
            this.btnBrowseInput.Size = new System.Drawing.Size(88, 88);
            this.btnBrowseInput.TabIndex = 0;
            this.btnBrowseInput.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnBrowseInput, "Browse");
            this.btnBrowseInput.UseVisualStyleBackColor = true;
            this.btnBrowseInput.Click += new System.EventHandler(this.btnBrowseInput_Click);
            // 
            // btnUpdateArchive1
            // 
            this.btnUpdateArchive1.FlatAppearance.BorderSize = 0;
            this.btnUpdateArchive1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateArchive1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdateArchive1.Location = new System.Drawing.Point(97, 3);
            this.btnUpdateArchive1.Name = "btnUpdateArchive1";
            this.btnUpdateArchive1.Size = new System.Drawing.Size(16, 88);
            this.btnUpdateArchive1.TabIndex = 0;
            this.btnUpdateArchive1.UseVisualStyleBackColor = true;
            this.btnUpdateArchive1.Click += new System.EventHandler(this.btnUpdateArchive1_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.66102F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.33898F));
            this.tableLayoutPanel5.Controls.Add(this.btnRemoveSelected, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 202);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(118, 94);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSelected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveSelected.Image = global::Crypt.Properties.Resources.Minus__3D__small_2;
            this.btnRemoveSelected.Location = new System.Drawing.Point(3, 3);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(88, 88);
            this.btnRemoveSelected.TabIndex = 0;
            this.btnRemoveSelected.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnRemoveSelected, "Remove selected");
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.03448F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.96552F));
            this.tableLayoutPanel6.Controls.Add(this.btnCompressEncrypt, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.btnCompressEncryptExtra, 1, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(121, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(116, 94);
            this.tableLayoutPanel6.TabIndex = 3;
            // 
            // btnCompressEncrypt
            // 
            this.btnCompressEncrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompressEncrypt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompressEncrypt.Image = ((System.Drawing.Image)(resources.GetObject("btnCompressEncrypt.Image")));
            this.btnCompressEncrypt.Location = new System.Drawing.Point(3, 3);
            this.btnCompressEncrypt.Name = "btnCompressEncrypt";
            this.btnCompressEncrypt.Size = new System.Drawing.Size(87, 88);
            this.btnCompressEncrypt.TabIndex = 0;
            this.btnCompressEncrypt.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnCompressEncrypt, "Compress & Encrypt");
            this.btnCompressEncrypt.UseVisualStyleBackColor = true;
            this.btnCompressEncrypt.Click += new System.EventHandler(this.btnCompressEncrypt_Click);
            // 
            // btnCompressEncryptExtra
            // 
            this.btnCompressEncryptExtra.FlatAppearance.BorderSize = 0;
            this.btnCompressEncryptExtra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCompressEncryptExtra.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompressEncryptExtra.Location = new System.Drawing.Point(96, 3);
            this.btnCompressEncryptExtra.Name = "btnCompressEncryptExtra";
            this.btnCompressEncryptExtra.Size = new System.Drawing.Size(16, 88);
            this.btnCompressEncryptExtra.TabIndex = 0;
            this.btnCompressEncryptExtra.Text = "▼";
            this.btnCompressEncryptExtra.UseVisualStyleBackColor = true;
            this.btnCompressEncryptExtra.Click += new System.EventHandler(this.btnCompressEncryptExtra_Click);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.9916F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.0084F));
            this.tableLayoutPanel7.Controls.Add(this.btnCheckKey, 0, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(121, 101);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(119, 94);
            this.tableLayoutPanel7.TabIndex = 4;
            // 
            // btnCheckKey
            // 
            this.btnCheckKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckKey.Enabled = false;
            this.btnCheckKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckKey.Image = ((System.Drawing.Image)(resources.GetObject("btnCheckKey.Image")));
            this.btnCheckKey.Location = new System.Drawing.Point(3, 3);
            this.btnCheckKey.Name = "btnCheckKey";
            this.btnCheckKey.Size = new System.Drawing.Size(88, 88);
            this.btnCheckKey.TabIndex = 0;
            this.btnCheckKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnCheckKey, "Check Key");
            this.btnCheckKey.UseVisualStyleBackColor = true;
            this.btnCheckKey.Click += new System.EventHandler(this.btnCheckKey_Click);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.9916F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.0084F));
            this.tableLayoutPanel8.Controls.Add(this.btnGenerateKey, 0, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(121, 202);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(3, 2, 0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(119, 94);
            this.tableLayoutPanel8.TabIndex = 5;
            // 
            // btnGenerateKey
            // 
            this.btnGenerateKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateKey.Enabled = false;
            this.btnGenerateKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateKey.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerateKey.Image")));
            this.btnGenerateKey.Location = new System.Drawing.Point(3, 3);
            this.btnGenerateKey.Name = "btnGenerateKey";
            this.btnGenerateKey.Size = new System.Drawing.Size(88, 88);
            this.btnGenerateKey.TabIndex = 0;
            this.btnGenerateKey.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnGenerateKey, "Generate Key");
            this.btnGenerateKey.UseVisualStyleBackColor = true;
            this.btnGenerateKey.Click += new System.EventHandler(this.btnGenerateKey_Click);
            // 
            // CryptForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(676, 576);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CryptForm";
            this.Text = "Needle in a Haystack in a Crypt";
            this.groupBoxChooseCryptionKey.ResumeLayout(false);
            this.groupBoxChooseCryptionKey.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxChooseCryptionAlgorithm.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBoxChooseCompressionLevel.ResumeLayout(false);
            this.groupBoxChooseCompressionLevel.PerformLayout();
            this.groupBoxOverwriteFiles.ResumeLayout(false);
            this.groupBoxOverwriteFiles.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBoxChooseOperatingMode.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TreeViewMS treeView1;
        private System.Windows.Forms.TextBox textBoxOutputFile;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxKeyFile;
        private System.Windows.Forms.Button btnBrowseKey;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.GroupBox groupBoxChooseCryptionKey;
        private System.Windows.Forms.RadioButton radioButtonGenerateNewKey;
        private System.Windows.Forms.RadioButton radioButtonUseExistingKey;
        private System.Windows.Forms.RadioButton radioButtonUseBuiltInDefault;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.GroupBox groupBoxChooseCryptionAlgorithm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBoxChooseCompressionLevel;
        private System.Windows.Forms.RadioButton radioButtonCompressionNone;
        private System.Windows.Forms.RadioButton radioButtonCompressionLow;
        private System.Windows.Forms.RadioButton radioButtonCompressionHigh;
        private System.Windows.Forms.GroupBox groupBoxOverwriteFiles;
        private System.Windows.Forms.RadioButton radioButtonOverwriteFilesNo;
        private System.Windows.Forms.RadioButton radioButtonOverwriteFilesYes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonOverwriteFilesAsk;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxPassword;
        private System.Windows.Forms.Button btnShowHidePassword;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBoxChooseOperatingMode;
        private System.Windows.Forms.RadioButton radioButtonDecompressDecrypt;
        private System.Windows.Forms.RadioButton radioButtonCompressEncrypt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnGenerateKey;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnCheckKey;
        private System.Windows.Forms.Button btnCompressEncrypt;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnShortcut;
        private System.Windows.Forms.ComboBox comboBoxChooseCryptionAlgorithm;
        private System.Windows.Forms.Button btnCompressEncryptExtra;
        private System.Windows.Forms.Button btnUpdateArchive1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.CheckBox checkBoxIsSplitArchive;
        private System.Windows.Forms.ComboBox comboBoxSplitArchiveSize;
    }
}

