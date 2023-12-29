namespace Crypt
{
    partial class OpenArchiveForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenArchiveForm));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.maskedTxtBox = new System.Windows.Forms.MaskedTextBox();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.btnBrowseKeyFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBoxKeyFile = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblAlgorithmName = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cryption key file:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.maskedTxtBox);
            this.groupBox1.Controls.Add(this.btnShowPassword);
            this.groupBox1.Controls.Add(this.btnBrowseKeyFile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtBoxKeyFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 142);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Please provide the key and/or password:";
            // 
            // maskedTxtBox
            // 
            this.maskedTxtBox.Location = new System.Drawing.Point(9, 96);
            this.maskedTxtBox.Name = "maskedTxtBox";
            this.maskedTxtBox.Size = new System.Drawing.Size(300, 20);
            this.maskedTxtBox.TabIndex = 6;
            this.maskedTxtBox.TextChanged += new System.EventHandler(this.maskedTxtBox_TextChanged);
            this.maskedTxtBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.maskedTxtBox_KeyUp);
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowPassword.Image = global::Crypt.Properties.Resources.Eye__3D__mini;
            this.btnShowPassword.Location = new System.Drawing.Point(315, 79);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(63, 53);
            this.btnShowPassword.TabIndex = 5;
            this.btnShowPassword.UseVisualStyleBackColor = true;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // btnBrowseKeyFile
            // 
            this.btnBrowseKeyFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseKeyFile.Image = global::Crypt.Properties.Resources.Plus__3D__mini;
            this.btnBrowseKeyFile.Location = new System.Drawing.Point(315, 25);
            this.btnBrowseKeyFile.Name = "btnBrowseKeyFile";
            this.btnBrowseKeyFile.Size = new System.Drawing.Size(63, 53);
            this.btnBrowseKeyFile.TabIndex = 4;
            this.btnBrowseKeyFile.UseVisualStyleBackColor = true;
            this.btnBrowseKeyFile.Click += new System.EventHandler(this.btnBrowseKeyFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // txtBoxKeyFile
            // 
            this.txtBoxKeyFile.AllowDrop = true;
            this.txtBoxKeyFile.Location = new System.Drawing.Point(9, 42);
            this.txtBoxKeyFile.Name = "txtBoxKeyFile";
            this.txtBoxKeyFile.Size = new System.Drawing.Size(300, 20);
            this.txtBoxKeyFile.TabIndex = 1;
            this.txtBoxKeyFile.TextChanged += new System.EventHandler(this.txtBoxKeyFile_TextChanged);
            this.txtBoxKeyFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtBoxKeyFile_DragDrop);
            this.txtBoxKeyFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtBoxKeyFile_DragEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblAlgorithmName);
            this.groupBox2.Location = new System.Drawing.Point(12, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 55);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "This archive uses the following algorithm:";
            // 
            // lblAlgorithmName
            // 
            this.lblAlgorithmName.AutoSize = true;
            this.lblAlgorithmName.Location = new System.Drawing.Point(12, 20);
            this.lblAlgorithmName.Name = "lblAlgorithmName";
            this.lblAlgorithmName.Size = new System.Drawing.Size(91, 13);
            this.lblAlgorithmName.TabIndex = 0;
            this.lblAlgorithmName.Text = "<Algorithm name>";
            // 
            // btnOpen
            // 
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Location = new System.Drawing.Point(321, 176);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 50);
            this.btnOpen.TabIndex = 3;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // OpenArchiveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(410, 243);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OpenArchiveForm";
            this.Text = "Open Archive";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnShowPassword;
        private System.Windows.Forms.Button btnBrowseKeyFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBoxKeyFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblAlgorithmName;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.MaskedTextBox maskedTxtBox;
    }
}