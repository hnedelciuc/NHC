namespace Crypt
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.comboBoxSelectionSetting = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxButtonSetting = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxPathModeSetting = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxWindowColorSetting = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxHighCompressionMode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBoxSelectionSetting
            // 
            this.comboBoxSelectionSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectionSetting.FormattingEnabled = true;
            this.comboBoxSelectionSetting.Location = new System.Drawing.Point(116, 17);
            this.comboBoxSelectionSetting.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxSelectionSetting.Name = "comboBoxSelectionSetting";
            this.comboBoxSelectionSetting.Size = new System.Drawing.Size(238, 21);
            this.comboBoxSelectionSetting.TabIndex = 0;
            this.comboBoxSelectionSetting.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectionSetting_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "For selection use:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 52);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "For buttons use:";
            // 
            // comboBoxButtonSetting
            // 
            this.comboBoxButtonSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxButtonSetting.FormattingEnabled = true;
            this.comboBoxButtonSetting.Location = new System.Drawing.Point(116, 49);
            this.comboBoxButtonSetting.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxButtonSetting.Name = "comboBoxButtonSetting";
            this.comboBoxButtonSetting.Size = new System.Drawing.Size(238, 21);
            this.comboBoxButtonSetting.TabIndex = 3;
            this.comboBoxButtonSetting.SelectedIndexChanged += new System.EventHandler(this.comboBoxButtonSetting_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(47, 277);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(99, 41);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(150, 277);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 41);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 201);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(345, 57);
            this.label3.TabIndex = 6;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // btnHelp
            // 
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Location = new System.Drawing.Point(216, 277);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(99, 41);
            this.btnHelp.TabIndex = 7;
            this.btnHelp.Text = "CmdLine Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Path mode:";
            // 
            // comboBoxPathModeSetting
            // 
            this.comboBoxPathModeSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPathModeSetting.FormattingEnabled = true;
            this.comboBoxPathModeSetting.Location = new System.Drawing.Point(116, 147);
            this.comboBoxPathModeSetting.Name = "comboBoxPathModeSetting";
            this.comboBoxPathModeSetting.Size = new System.Drawing.Size(238, 21);
            this.comboBoxPathModeSetting.TabIndex = 9;
            this.comboBoxPathModeSetting.SelectedIndexChanged += new System.EventHandler(this.comboBoxPathModeSetting_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Window color:";
            // 
            // comboBoxWindowColorSetting
            // 
            this.comboBoxWindowColorSetting.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWindowColorSetting.FormattingEnabled = true;
            this.comboBoxWindowColorSetting.Location = new System.Drawing.Point(116, 81);
            this.comboBoxWindowColorSetting.Name = "comboBoxWindowColorSetting";
            this.comboBoxWindowColorSetting.Size = new System.Drawing.Size(238, 21);
            this.comboBoxWindowColorSetting.TabIndex = 11;
            this.comboBoxWindowColorSetting.SelectedIndexChanged += new System.EventHandler(this.comboBoxWindowColorSetting_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(16, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 29);
            this.label6.TabIndex = 12;
            this.label6.Text = "High Compression Mode:";
            // 
            // comboBoxHighCompressionMode
            // 
            this.comboBoxHighCompressionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxHighCompressionMode.FormattingEnabled = true;
            this.comboBoxHighCompressionMode.Location = new System.Drawing.Point(116, 112);
            this.comboBoxHighCompressionMode.Name = "comboBoxHighCompressionMode";
            this.comboBoxHighCompressionMode.Size = new System.Drawing.Size(238, 21);
            this.comboBoxHighCompressionMode.TabIndex = 13;
            this.comboBoxHighCompressionMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxHighCompressionMode_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(370, 336);
            this.Controls.Add(this.comboBoxHighCompressionMode);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxWindowColorSetting);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxPathModeSetting);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.comboBoxButtonSetting);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxSelectionSetting);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSelectionSetting;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxButtonSetting;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxPathModeSetting;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxWindowColorSetting;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxHighCompressionMode;
    }
}