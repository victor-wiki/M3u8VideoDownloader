using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoHandler
{
    public class M3u8Info
    {
        public List<M3u8LineInfo> Lines = new List<M3u8LineInfo>();        
        public M3u8KeyInfo Key { get; set; }
        public List<M3u8PlayItemInfo> PlayList { get; set; } = new List<M3u8PlayItemInfo>();
    }

    public class M3u8LineInfo
    {
        public int Index { get; set; }
        public string Content { get; set; }
        public M3u8LineType Type { get; set; }
    }

    public class M3u8PlayItemInfo
    {
        public int Index { get; set; }
        public int LineIndex { get; set; }
        public double Duration { get; set; }
        public string Path { get; set; }
    }   

    public class M3u8KeyInfo
    {
        public string Method { get; set; }
        public string Url { get; set; }
    }

    public enum M3u8LineType
    {
        Key,
        PlayItemPrefix,
        PlayItemContent,
        Other
    }
}
