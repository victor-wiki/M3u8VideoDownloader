using M3u8VideoHandler;
using System.IO;
using System.Windows.Forms;

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
