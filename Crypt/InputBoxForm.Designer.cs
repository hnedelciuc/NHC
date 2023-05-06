namespace Crypt
{
    partial class InputBoxForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputBoxForm));
            this.label = new System.Windows.Forms.Label();
            this.textBoxOutputFile = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNoToAll = new System.Windows.Forms.Button();
            this.btnYesToAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(10, 20);
            this.label.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(312, 100);
            this.label.TabIndex = 0;
            this.label.Text = "label";
            // 
            // textBoxOutputFile
            // 
            this.textBoxOutputFile.Location = new System.Drawing.Point(10, 74);
            this.textBoxOutputFile.Margin = new System.Windows.Forms.Padding(1);
            this.textBoxOutputFile.Name = "textBoxOutputFile";
            this.textBoxOutputFile.Size = new System.Drawing.Size(233, 20);
            this.textBoxOutputFile.TabIndex = 1;
            this.textBoxOutputFile.AllowDrop = true;
            this.textBoxOutputFile.Leave += new System.EventHandler(textBoxOutputFile_Leave);
            this.textBoxOutputFile.DragEnter += new System.Windows.Forms.DragEventHandler(textBoxOutputFile_DragEnter);
            this.textBoxOutputFile.DragDrop += new System.Windows.Forms.DragEventHandler(textBoxOutputFile_DragDrop);
            // 
            // btnOk
            // 
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(168, 134);
            this.btnOk.Margin = new System.Windows.Forms.Padding(1);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 64);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.Image")));
            this.btnBrowse.Location = new System.Drawing.Point(247, 51);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 64);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(247, 134);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 64);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNoToAll
            // 
            this.btnNoToAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNoToAll.Location = new System.Drawing.Point(89, 134);
            this.btnNoToAll.Name = "btnNoToAll";
            this.btnNoToAll.Size = new System.Drawing.Size(75, 64);
            this.btnNoToAll.TabIndex = 5;
            this.btnNoToAll.Text = "No to All";
            this.btnNoToAll.UseVisualStyleBackColor = true;
            this.btnNoToAll.Click += new System.EventHandler(this.btnNoToAll_Click);
            // 
            // btnYesToAll
            // 
            this.btnYesToAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYesToAll.Location = new System.Drawing.Point(10, 134);
            this.btnYesToAll.Name = "btnYesToAll";
            this.btnYesToAll.Size = new System.Drawing.Size(75, 64);
            this.btnYesToAll.TabIndex = 6;
            this.btnYesToAll.Text = "Yes to All";
            this.btnYesToAll.UseVisualStyleBackColor = true;
            this.btnYesToAll.Click += new System.EventHandler(this.btnYesToAll_Click);
            // 
            // InputBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(334, 210);
            this.Controls.Add(this.btnYesToAll);
            this.Controls.Add(this.btnNoToAll);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.textBoxOutputFile);
            this.Controls.Add(this.label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.Name = "InputBoxForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TextBox textBoxOutputFile;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNoToAll;
        private System.Windows.Forms.Button btnYesToAll;
    }
}