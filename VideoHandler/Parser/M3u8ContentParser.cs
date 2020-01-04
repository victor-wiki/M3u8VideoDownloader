using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace VideoHandler
{
    public class M3u8ContentParser
    {
        public string FilePath { get; set; }
        public M3u8ContentParser(string filePath)
        {
            this.FilePath = filePath;
        }

        public M3u8Info Parse()
        {
            M3u8Info info = new M3u8Info();

            string[] lines = null;

            if (File.Exists(this.FilePath))
            {
                lines = File.ReadAllLines(this.FilePath);
            }
            else if (UrlHelper.IsValidUrl(this.FilePath))
            {
                using (WebClient wc = new WebClient())
                {
                    string content = wc.DownloadString(this.FilePath);
                    lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if(lines!=null)
            {
                int i = 0;
                List<int> playListIndex = new List<int>();
                foreach (string line in lines)
                {
                    M3u8LineInfo lineInfo = new M3u8LineInfo() { Index = i, Content = line };

                    if (line.StartsWith("#EXT-X-KEY"))
                    {
                        lineInfo.Type = M3u8LineType.Key;

                        M3u8KeyInfo keyInfo = new M3u8KeyInfo();
                        string[] items = line.Replace("#EXT-X-KEY:", "").Split(',');
                        foreach (string item in items)
                        {
                            string[] kps = item.Split('=');
                            string key = kps[0];
                            string value = kps[1];
                            if (key == "METHOD")
                            {
                                keyInfo.Method = value;
                            }
                            else if (key == "URI")
                            {
                                keyInfo.Url = value.Trim('"');
                            }
                        }

                        info.Key = keyInfo;
                    }
                    else if (line.StartsWith("#EXTINF"))
                    {
                        lineInfo.Type = M3u8LineType.PlayItemPrefix;
                        string value = line.Split(':')[1].Trim(',');
                        double duration = 0;
                        if (double.TryParse(value, out duration) && i < lines.Length)
                        {
                            info.PlayList.Add(new M3u8PlayItemInfo() { Index = playListIndex.Count(), LineIndex = i + 1, Duration = duration, Path = lines[i + 1] });
                            playListIndex.Add(i + 1);
                        }
                    }
                    else if (playListIndex.Contains(i))
                    {
                        lineInfo.Type = M3u8LineType.PlayItemContent;
                    }

                    info.Lines.Add(lineInfo);

                    i++;
                }
            }            

            return info;
        }
    }
}
