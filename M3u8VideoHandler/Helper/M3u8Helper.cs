using System.Collections.Generic;
using System.Linq;

namespace M3u8VideoHandler
{
    public class M3u8Helper
    {
        public static M3u8Info UpdateContent(M3u8Info info, M3u8KeyInfo keyInfo, List<M3u8PlayItemInfo> playList)
        {
            if(keyInfo!=null)
            {
                M3u8LineInfo lineInfo = info.Lines.FirstOrDefault(item => item.Type == M3u8LineType.Key);
                if(lineInfo!=null)
                {
                    lineInfo.Content = $"#EXT-X-KEY:METHOD={keyInfo.Method},URI=\"{keyInfo.Url}\"";
                }
            }

            if (playList!=null)
            {
                foreach(M3u8PlayItemInfo item in playList)
                {
                    int lineIndex = item.LineIndex;
                    M3u8LineInfo lineInfo = info.Lines.FirstOrDefault(l=>l.Index==lineIndex);
                    if(lineInfo!=null)
                    {
                        lineInfo.Content = item.Path;
                    }
                }
            }

            return info;
        }
    }
}
