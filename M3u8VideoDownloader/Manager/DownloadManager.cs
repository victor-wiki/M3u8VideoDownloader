using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoHandler;

namespace M3u8VideoDownloader
{
    public class DownloadManager
    {
        private static DownloadSetting setting = SettingManager.Setting;
        private static frmDownloader downloader;

        public static frmDownloader Downloder
        {
            get
            {
                if(downloader==null || downloader.IsDisposed)
                {
                    downloader = new frmDownloader();
                }
                return downloader;
            }
        }

        public static frmDownloader ShowDownloader()
        {
            Downloder.Show();
            Downloder.BringToFront();
            return Downloder;
        }

        public static string DefaultSaveFolder
        {           
            get
            {
                return string.IsNullOrEmpty(setting.SaveFolder) ? Path.GetDirectoryName(Application.ExecutablePath) : setting.SaveFolder;
            }           
        }
    }
}
