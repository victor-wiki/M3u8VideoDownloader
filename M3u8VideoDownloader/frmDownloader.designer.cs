namespace M3u8VideoDownloader
{
    partial class frmDownloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDownloader));
            this.lvMessage = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStartTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkShutdownAfterDownload = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lvMessage
            // 
            this.lvMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colMessage,
            this.colStartTime});
            this.lvMessage.FullRowSelect = true;
            this.lvMessage.GridLines = true;
            this.lvMessage.HideSelection = false;
            this.lvMessage.Location = new System.Drawing.Point(0, 0);
            this.lvMessage.Name = "lvMessage";
            this.lvMessage.Size = new System.Drawing.Size(813, 260);
            this.lvMessage.TabIndex = 17;
            this.lvMessage.UseCompatibleStateImageBehavior = false;
            this.lvMessage.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 200;
            // 
            // colMessage
            // 
            this.colMessage.Text = "Message";
            this.colMessage.Width = 500;
            // 
            // colStartTime
            // 
            this.colStartTime.Text = "Start Time";
            this.colStartTime.Width = 100;
            // 
            // chkShutdownAfterDownload
            // 
            this.chkShutdownAfterDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShutdownAfterDownload.AutoSize = true;
            this.chkShutdownAfterDownload.Location = new System.Drawing.Point(669, 266);
            this.chkShutdownAfterDownload.Name = "chkShutdownAfterDownload";
            this.chkShutdownAfterDownload.Size = new System.Drawing.Size(138, 16);
            this.chkShutdownAfterDownload.TabIndex = 18;
            this.chkShutdownAfterDownload.Text = "Shutdown PC if done";
            this.chkShutdownAfterDownload.UseVisualStyleBackColor = true;
            // 
            // frmDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 284);
            this.Controls.Add(this.chkShutdownAfterDownload);
            this.Controls.Add(this.lvMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDownloader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download message";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDownloader_FormClosing);
            this.Load += new System.EventHandler(this.frmDownloader_Load);
            this.SizeChanged += new System.EventHandler(this.frmDownloader_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvMessage;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colMessage;
        private System.Windows.Forms.ColumnHeader colStartTime;
        private System.Windows.Forms.CheckBox chkShutdownAfterDownload;
    }
}