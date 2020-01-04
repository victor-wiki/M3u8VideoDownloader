using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoHandler;
using System.IO;
using Newtonsoft.Json;

namespace M3u8VideoDownloader
{
    public class SettingManager
    {
        private const string settingFileName = "setting.json";
        private static DownloadSetting _setting;

        public static DownloadSetting Setting
        {
            get
            {
                if(_setting==null)
                {
                    return GetSetting();
                }
                return _setting;
            }

        }

        public static DownloadSetting GetSetting()
        {
            if(File.Exists(settingFileName))
            {
                string content = File.ReadAllText(settingFileName);
                DownloadSetting setting = (DownloadSetting) JsonConvert.DeserializeObject(content, typeof(DownloadSetting));
                return setting;
            }
            else
            {
                return new DownloadSetting();
            }
        }

        public static void SaveSetting(DownloadSetting setting)
        {
            _setting = setting;
            string content = JsonConvert.SerializeObject(setting, Formatting.Indented);
            File.WriteAllText(settingFileName, content);
        }
    }
}
