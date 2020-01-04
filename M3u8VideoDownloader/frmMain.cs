using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M3u8VideoHandler;

namespace M3u8VideoDownloader
{
    public partial class frmMain : Form
    {
        private DownloadSetting setting = SettingManager.Setting;
        private int initColumnWidthExludeUrl = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.InitControls();
        }

        private void tsmSetting_Click(object sender, EventArgs e)
        {
            frmSetting setting = new frmSetting();
            setting.ShowDialog();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = "";
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.AddUrls(this.openFileDialog1.FileNames);
            }
        }

        private void btnCloudUrl_Click(object sender, EventArgs e)
        {
            frmTextContent frmTextContent = new frmTextContent();
            DialogResult result = frmTextContent.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (frmTextContent.Content != null)
                {
                    string[] urls = frmTextContent.Content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (urls.Length > 0)
                    {
                        this.AddUrls(urls);
                    }
                }
            }
        }

        private void AddUrls(IEnumerable<string> urls)
        {
            List<string> existedUrls = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string url = dataGridView1.Rows[i].Cells["Url"].Value?.ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    existedUrls.Add(url);
                }
            }

            foreach (string url in urls)
            {
                if (!existedUrls.Contains(url) && UrlHelper.IsM3u8(url))
                {
                    int order = this.dataGridView1.Rows.Count + 1;
                    bool isValidUrl = UrlHelper.IsValidUrl(url);

                    string fileName = "";

                    if (this.chkUseNoAsFileName.Checked)
                    {
                        fileName = order.ToString();
                    }
                    else
                    {
                        fileName = isValidUrl ? Path.GetFileNameWithoutExtension(new Uri(url).LocalPath) : (File.Exists(url) ? Path.GetFileNameWithoutExtension(url) : order.ToString());
                    } 

                    this.dataGridView1.Rows.Add(order, fileName, url, isValidUrl ? "Play" : "");
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.Columns[e.ColumnIndex].Name == "Play")
            {
                string value = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                string name = this.dataGridView1.Rows[e.RowIndex].Cells["_Name"].Value.ToString();
                string url = this.dataGridView1.Rows[e.RowIndex].Cells["Url"].Value?.ToString();
                if (UrlHelper.IsValidUrl(url))
                {
                    Process.Start($"https://bharadwajpro.github.io/m3u8-player/player/#{url}");
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            this.Download();
        }

        private async Task Download()
        {
            try
            {
                List<string> existedNames = new List<string>();
                List<string> existedUrls = new List<string>();

                List<VideoInfo> videoInfos = new List<VideoInfo>();

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string name = dataGridView1.Rows[i].Cells["_Name"].Value?.ToString();
                    string url = dataGridView1.Rows[i].Cells["Url"].Value?.ToString();
                    if ((!string.IsNullOrEmpty(name) && existedNames.Contains(name))
                        || (!string.IsNullOrEmpty(url) && existedUrls.Contains(url)))
                    {
                        MessageBox.Show("Name and url can't be duplicated.");
                        return;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        existedNames.Add(name);
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        existedUrls.Add(url);
                    }

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url) && (UrlHelper.IsValidUrl(url) || File.Exists(url)))
                    {
                        videoInfos.Add(new VideoInfo() { Name = name, Url = url });
                    }
                }

                string saveFolder = this.txtSaveFolder.Text.Trim();
                if (string.IsNullOrEmpty(saveFolder))
                {
                    string downloadFolder = string.IsNullOrEmpty(setting.SaveFolder) ? Path.GetDirectoryName(Application.ExecutablePath) : setting.SaveFolder;
                    if (!Directory.Exists(downloadFolder))
                    {
                        Directory.CreateDirectory(downloadFolder);
                    }
                    saveFolder = Path.Combine(downloadFolder, DateTime.Now.ToString("yyyyMMddHHmm"));
                    this.txtSaveFolder.Text = saveFolder;
                }

                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }

                await DownloadManager.ShowDownloader().Download(saveFolder, videoInfos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtSaveFolder.Text))
            {
                this.folderBrowserDialog1.SelectedPath = this.txtSaveFolder.Text;
            }

            DialogResult result = this.folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.txtSaveFolder.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void InitControls()
        {
            if(!string.IsNullOrEmpty(this.setting.SaveFolder))
            {
                this.txtSaveFolder.Text = this.setting.SaveFolder;
            }

            foreach (DataGridViewColumn col in this.dataGridView1.Columns)
            {
                if (col.Name != "Url")
                {
                    this.initColumnWidthExludeUrl += col.Width;
                }
            }
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            this.dataGridView1.Columns["Url"].Width = this.dataGridView1.Width - this.initColumnWidthExludeUrl - 10;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.dataGridView1.SelectedRows!=null)
            {
                List<int> indexes = new List<int>();
                foreach(DataGridViewRow row in this.dataGridView1.SelectedRows)
                {
                    indexes.Add(row.Index);
                }
                indexes.Reverse();
                indexes.ForEach(item => this.dataGridView1.Rows.RemoveAt(item));               
            }
        }

        private void clearRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
        }
    }
}
