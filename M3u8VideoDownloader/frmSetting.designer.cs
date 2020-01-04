namespace M3u8VideoDownloader
{
    partial class frmSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetting));
            this.label1 = new System.Windows.Forms.Label();
            this.txtSaveFolder = new System.Windows.Forms.TextBox();
            this.btnOpenSaveFolder = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkKeepTsFile = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbRemote = new System.Windows.Forms.RadioButton();
            this.rbLocal = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.chkKeepM3u8File = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkEnableDebug = new System.Windows.Forms.CheckBox();
            this.chkEnableLog = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudConnectLimit = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConnectLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default save folder:";
            // 
            // txtSaveFolder
            // 
            this.txtSaveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSaveFolder.Location = new System.Drawing.Point(155, 10);
            this.txtSaveFolder.Name = "txtSaveFolder";
            this.txtSaveFolder.Size = new System.Drawing.Size(441, 21);
            this.txtSaveFolder.TabIndex = 1;
            // 
            // btnOpenSaveFolder
            // 
            this.btnOpenSaveFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenSaveFolder.Location = new System.Drawing.Point(602, 9);
            this.btnOpenSaveFolder.Name = "btnOpenSaveFolder";
            this.btnOpenSaveFolder.Size = new System.Drawing.Size(36, 23);
            this.btnOpenSaveFolder.TabIndex = 2;
            this.btnOpenSaveFolder.Text = "...";
            this.btnOpenSaveFolder.UseVisualStyleBackColor = true;
            this.btnOpenSaveFolder.Click += new System.EventHandler(this.btnOpenSaveFolder_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(474, 252);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(555, 252);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkKeepTsFile
            // 
            this.chkKeepTsFile.AutoSize = true;
            this.chkKeepTsFile.Location = new System.Drawing.Point(189, 56);
            this.chkKeepTsFile.Name = "chkKeepTsFile";
            this.chkKeepTsFile.Size = new System.Drawing.Size(114, 16);
            this.chkKeepTsFile.TabIndex = 6;
            this.chkKeepTsFile.Text = "Reserve ts file";
            this.chkKeepTsFile.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRemote);
            this.groupBox1.Controls.Add(this.rbLocal);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkKeepM3u8File);
            this.groupBox1.Controls.Add(this.chkKeepTsFile);
            this.groupBox1.Location = new System.Drawing.Point(17, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(621, 83);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "m3u8";
            // 
            // rbRemote
            // 
            this.rbRemote.AutoSize = true;
            this.rbRemote.Location = new System.Drawing.Point(132, 21);
            this.rbRemote.Name = "rbRemote";
            this.rbRemote.Size = new System.Drawing.Size(59, 16);
            this.rbRemote.TabIndex = 10;
            this.rbRemote.Text = "Remote";
            this.rbRemote.UseVisualStyleBackColor = true;
            this.rbRemote.CheckedChanged += new System.EventHandler(this.rbRemote_CheckedChanged);
            // 
            // rbLocal
            // 
            this.rbLocal.AutoSize = true;
            this.rbLocal.Checked = true;
            this.rbLocal.Location = new System.Drawing.Point(60, 21);
            this.rbLocal.Name = "rbLocal";
            this.rbLocal.Size = new System.Drawing.Size(53, 16);
            this.rbLocal.TabIndex = 9;
            this.rbLocal.TabStop = true;
            this.rbLocal.Text = "Local";
            this.rbLocal.UseVisualStyleBackColor = true;
            this.rbLocal.CheckedChanged += new System.EventHandler(this.rbLocal_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Mode:";
            // 
            // chkKeepM3u8File
            // 
            this.chkKeepM3u8File.AutoSize = true;
            this.chkKeepM3u8File.Location = new System.Drawing.Point(60, 56);
            this.chkKeepM3u8File.Name = "chkKeepM3u8File";
            this.chkKeepM3u8File.Size = new System.Drawing.Size(126, 16);
            this.chkKeepM3u8File.TabIndex = 7;
            this.chkKeepM3u8File.Text = "Reserve m3u8 file";
            this.chkKeepM3u8File.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkEnableDebug);
            this.groupBox2.Controls.Add(this.chkEnableLog);
            this.groupBox2.Location = new System.Drawing.Point(17, 171);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(621, 67);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log";
            // 
            // chkEnableDebug
            // 
            this.chkEnableDebug.AutoSize = true;
            this.chkEnableDebug.Checked = true;
            this.chkEnableDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableDebug.Location = new System.Drawing.Point(123, 30);
            this.chkEnableDebug.Name = "chkEnableDebug";
            this.chkEnableDebug.Size = new System.Drawing.Size(96, 16);
            this.chkEnableDebug.TabIndex = 1;
            this.chkEnableDebug.Text = "Enable debug";
            this.chkEnableDebug.UseVisualStyleBackColor = true;
            // 
            // chkEnableLog
            // 
            this.chkEnableLog.AutoSize = true;
            this.chkEnableLog.Checked = true;
            this.chkEnableLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableLog.Location = new System.Drawing.Point(12, 31);
            this.chkEnableLog.Name = "chkEnableLog";
            this.chkEnableLog.Size = new System.Drawing.Size(84, 16);
            this.chkEnableLog.TabIndex = 0;
            this.chkEnableLog.Text = "Enable log";
            this.chkEnableLog.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "Max connection number:";
            // 
            // nudConnectLimit
            // 
            this.nudConnectLimit.Location = new System.Drawing.Point(155, 40);
            this.nudConnectLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudConnectLimit.Name = "nudConnectLimit";
            this.nudConnectLimit.Size = new System.Drawing.Size(78, 21);
            this.nudConnectLimit.TabIndex = 10;
            this.nudConnectLimit.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 287);
            this.Controls.Add(this.nudConnectLimit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnOpenSaveFolder);
            this.Controls.Add(this.txtSaveFolder);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudConnectLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSaveFolder;
        private System.Windows.Forms.Button btnOpenSaveFolder;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkKeepTsFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkEnableLog;
        private System.Windows.Forms.CheckBox chkEnableDebug;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudConnectLimit;
        private System.Windows.Forms.RadioButton rbRemote;
        private System.Windows.Forms.RadioButton rbLocal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkKeepM3u8File;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}