using System.IO;
using System.Text;

namespace VideoHandler
{
    public class M3u8Writer
    {
        public M3u8Info Info { get; set; }
        public M3u8Writer(M3u8Info info)
        {
            this.Info = info;
        }

        public void Write(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            foreach(M3u8LineInfo lineInfo in this.Info.Lines)
            {
                sb.AppendLine(lineInfo.Content);
            }           

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
