using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using M3u8VideoHandler;

namespace M3u8VideoDownloader
{
    public partial class frmDownloader : Form, IObserver<FeedbackInfo>
    {
        private DownloadSetting setting;
        private int initColumnWidthExludeMessage = 0;

        public List<VideoInfo> VideoInfos { get; private set; } = new List<VideoInfo>();
        public List<Downloader> Downloaders { get; private set; } = new List<Downloader>();

        public frmDownloader()
        {
            InitializeComponent();
            ListView.CheckForIllegalCrossThreadCalls = false;
            Form.CheckForIllegalCrossThreadCalls = false;
        }

        private void frmDownloader_Load(object sender, EventArgs e)
        {
            this.InitControls();
        }

        private void InitControls()
        {
            foreach (ColumnHeader col in this.lvMessage.Columns)
            {
                if (col.DisplayIndex != 1)
                {
                    this.initColumnWidthExludeMessage += col.Width;
                }
            }
        }

        private void LoadSetting()
        {
            this.setting = SettingManager.Setting;
            ServicePointManager.DefaultConnectionLimit = setting.ConnectLimitNum;
            FeedbackHelper.EnableLog = setting.EnableLog;
            LogHelper.EnableDebug = setting.EnableDebug;
        }

        public async Task Download(string saveFolder, List<VideoInfo> videoInfos)
        {
            this.LoadSetting();

            this.VideoInfos.AddRange(videoInfos);

            Downloader downloader = new Downloader(saveFolder, setting);

            this.Downloaders.Add(downloader);

            downloader.Subscribe(this);

            try
            {
                foreach (var video in videoInfos)
                {                                   
                    if (!this.lvMessage.Items.ContainsKey(video.Name))
                    {
                        ListViewItem li = new ListViewItem();
                        li.Tag = video;
                        li.Name = video.Name;
                        li.Text = video.Name;
                        li.SubItems.Add("Ready");
                        li.SubItems.Add("");
                        li.SubItems.Add("");

                        this.lvMessage.Items.Add(li);
                    }
                    else
                    {
                        ListViewItem li = this.lvMessage.Items[video.Name];
                        li.Tag = video;
                        li.SubItems[1].Text = "Ready";
                        li.SubItems[2].Text = "";
                        li.SubItems[2].Tag = null;
                        li.SubItems[3].Text = "";
                        li.BackColor = Color.White;
                    }
                }               

                await downloader.Download(videoInfos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnNext(FeedbackInfo feedback)
        {
            VideoInfo video = feedback.Source as VideoInfo;

            if (this.IsDisposed)
            {
                return;
            }

            Action showResult = () =>
             {
                 if (video != null)
                 {
                     int finishCount = 0;
                     foreach (ListViewItem item in this.lvMessage.Items)
                     {
                         if (item.Text == video.Name)
                         {
                             VideoInfo v = item.Tag as VideoInfo;
                             if(v!=null)
                             {
                                 v.TaskState = video.TaskState;
                             }

                             item.SubItems[1].Text = feedback.Content;

                             if (item.SubItems[2].Tag == null)
                             {                                
                                 item.SubItems[2].Text = DateTime.Now.ToString("HH:mm:ss");
                                 item.SubItems[2].Tag = DateTime.Now;
                             }                           

                             if (video.TaskState==DownloadTaskState.Finished)
                             {
                                 finishCount++;
                                 item.BackColor = Color.LightGreen;
                             }
                             else if(video.TaskState==DownloadTaskState.Error)
                             {
                                 item.BackColor = Color.Pink;
                             }
                             else
                             {
                                 item.BackColor = Color.White;
                             }                            
                         }
                     }

                     if (this.chkShutdownAfterDownload.Checked && finishCount == this.lvMessage.Items.Count)
                     {
                         this.Shutdown();
                         return;
                     }

                     this.lvMessage.Refresh();
                 }
             };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(showResult);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }
            else
            {
                showResult();
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        public bool IsBusy
        {
            get
            {
                return this.Downloaders.Any(item => item != null && item.IsBusy);
            }
        }

        public void Cancel()
        {
            foreach (Downloader downloader in this.Downloaders)
            {
                if (downloader != null)
                {
                    downloader.Cancel();
                }
            }
        }

        private void frmDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.IsBusy)
            {
                DialogResult result = MessageBox.Show("There is running task, are you sure to exit?", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = false;
                    this.Cancel();
                    this.Clear();
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                this.Clear();
            }
        }

        private void Clear()
        {
            this.VideoInfos.Clear();
            this.Downloaders.Clear();
        }    

        private void Shutdown()
        {
            Process process = new Process();
            process.StartInfo.FileName = "shutdown";
            process.StartInfo.Arguments = "/s";
            process.Start();
        }

        private void frmDownloader_SizeChanged(object sender, EventArgs e)
        {
            this.lvMessage.Columns[1].Width = this.lvMessage.Width - this.initColumnWidthExludeMessage - 10;
        }
    }
}
