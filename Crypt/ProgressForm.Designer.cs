using System;
using System.Windows.Forms;

namespace Crypt
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelCompressEncrypt = new System.Windows.Forms.Label();
            this.labelPercentage = new System.Windows.Forms.Label();
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.labelResultMessage = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutPanelProgress = new System.Windows.Forms.TableLayoutPanel();
            this.labelElapsedTime = new System.Windows.Forms.Label();
            this.tableLayoutPanelProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(19, 34);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(308, 19);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 29;
            // 
            // labelCompressEncrypt
            // 
            this.labelCompressEncrypt.AutoSize = true;
            this.labelCompressEncrypt.Location = new System.Drawing.Point(21, 18);
            this.labelCompressEncrypt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCompressEncrypt.Name = "labelCompressEncrypt";
            this.labelCompressEncrypt.Size = new System.Drawing.Size(61, 13);
            this.labelCompressEncrypt.TabIndex = 30;
            this.labelCompressEncrypt.Text = "                  ";
            // 
            // labelPercentage
            // 
            this.labelPercentage.AutoSize = true;
            this.labelPercentage.Location = new System.Drawing.Point(301, 18);
            this.labelPercentage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPercentage.Name = "labelPercentage";
            this.labelPercentage.Size = new System.Drawing.Size(22, 13);
            this.labelPercentage.TabIndex = 31;
            this.labelPercentage.Text = "     ";
            this.labelPercentage.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // worker
            // 
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.worker_DoWork);
            this.worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.worker_RunWorkerCompleted);
            // 
            // labelResultMessage
            // 
            this.labelResultMessage.AutoSize = true;
            this.labelResultMessage.Location = new System.Drawing.Point(2, 29);
            this.labelResultMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelResultMessage.MaximumSize = new System.Drawing.Size(308, 203);
            this.labelResultMessage.Name = "labelResultMessage";
            this.labelResultMessage.Size = new System.Drawing.Size(34, 13);
            this.labelResultMessage.TabIndex = 32;
            this.labelResultMessage.Text = "         ";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClose.Location = new System.Drawing.Point(126, 60);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(56, 24);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderSize = 1;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tableLayoutPanelProgress
            // 
            this.tableLayoutPanelProgress.AutoSize = true;
            this.tableLayoutPanelProgress.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelProgress.ColumnCount = 1;
            this.tableLayoutPanelProgress.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelProgress.Controls.Add(this.labelElapsedTime, 0, 0);
            this.tableLayoutPanelProgress.Controls.Add(this.btnClose, 0, 4);
            this.tableLayoutPanelProgress.Controls.Add(this.labelResultMessage, 0, 2);
            this.tableLayoutPanelProgress.Location = new System.Drawing.Point(19, 57);
            this.tableLayoutPanelProgress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanelProgress.MaximumSize = new System.Drawing.Size(308, 0);
            this.tableLayoutPanelProgress.MinimumSize = new System.Drawing.Size(308, 0);
            this.tableLayoutPanelProgress.Name = "tableLayoutPanelProgress";
            this.tableLayoutPanelProgress.RowCount = 5;
            this.tableLayoutPanelProgress.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelProgress.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanelProgress.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelProgress.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanelProgress.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelProgress.Size = new System.Drawing.Size(308, 86);
            this.tableLayoutPanelProgress.TabIndex = 34;
            // 
            // labelElapsedTime
            // 
            this.labelElapsedTime.AutoSize = true;
            this.labelElapsedTime.Location = new System.Drawing.Point(2, 0);
            this.labelElapsedTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelElapsedTime.Name = "labelElapsedTime";
            this.labelElapsedTime.Size = new System.Drawing.Size(28, 13);
            this.labelElapsedTime.TabIndex = 34;
            this.labelElapsedTime.Text = "       ";
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(346, 149);
            this.Controls.Add(this.tableLayoutPanelProgress);
            this.Controls.Add(this.labelPercentage);
            this.Controls.Add(this.labelCompressEncrypt);
            this.Controls.Add(this.progressBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "ProgressForm";
            this.Text = "Progress...";
            this.Load += new System.EventHandler(this.Progress_Load);
            this.tableLayoutPanelProgress.ResumeLayout(false);
            this.tableLayoutPanelProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelCompressEncrypt;
        private System.Windows.Forms.Label labelPercentage;
        private System.ComponentModel.BackgroundWorker worker;
        private Label labelResultMessage;
        private Button btnClose;
        private TableLayoutPanel tableLayoutPanelProgress;
        private Label labelElapsedTime;
    }
}