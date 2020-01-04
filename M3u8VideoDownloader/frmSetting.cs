using M3u8VideoHandler;
using System;
using System.Windows.Forms;

namespace M3u8VideoDownloader
{
    public partial class frmSetting : Form
    {
        public frmSetting()
        {
            InitializeComponent();
        }       

        private void frmSetting_Load(object sender, EventArgs e)
        {
            DownloadSetting setting = SettingManager.GetSetting();
            this.txtSaveFolder.Text = setting.SaveFolder;           
            this.rbLocal.Checked = setting.M3u8ProcessMode == M3u8ProcessMode.Local;
            this.rbRemote.Checked = setting.M3u8ProcessMode == M3u8ProcessMode.Remote;
            this.chkKeepM3u8File.Checked = setting.KeepM3u8File;
            this.chkKeepTsFile.Checked = setting.KeepTsFile;
            this.chkEnableLog.Checked = setting.EnableLog;
            this.chkEnableDebug.Checked = setting.EnableDebug;
            this.nudConnectLimit.Value = setting.ConnectLimitNum;

            this.SetControlState();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DownloadSetting setting = new DownloadSetting();
            setting.SaveFolder = this.txtSaveFolder.Text;           
            setting.M3u8ProcessMode = this.rbLocal.Checked ? M3u8ProcessMode.Local : M3u8ProcessMode.Remote;
            setting.KeepM3u8File = this.chkKeepM3u8File.Checked;
            setting.KeepTsFile = this.chkKeepTsFile.Checked;
            setting.EnableLog = this.chkEnableLog.Checked;
            setting.EnableDebug = this.chkEnableDebug.Checked;
            setting.ConnectLimitNum =(int) this.nudConnectLimit.Value;
            SettingManager.SaveSetting(setting);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOpenSaveFolder_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.txtSaveFolder.Text))
            {
                this.folderBrowserDialog1.SelectedPath = this.txtSaveFolder.Text;
            }

            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if(result==DialogResult.OK)
            {
                this.txtSaveFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void rbLocal_CheckedChanged(object sender, EventArgs e)
        {
            this.SetControlState();
        }

        private void rbRemote_CheckedChanged(object sender, EventArgs e)
        {
            this.SetControlState();
        }

        private void SetControlState()
        {
            this.chkKeepTsFile.Enabled = this.rbLocal.Checked;
            this.chkKeepM3u8File.Enabled = this.rbLocal.Checked;

            if(!this.rbLocal.Checked)
            {
                this.chkKeepTsFile.Checked = false;
                this.chkKeepM3u8File.Checked = false;
            }
        }
    }
}
