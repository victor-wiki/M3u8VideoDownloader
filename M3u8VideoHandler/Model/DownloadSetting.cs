namespace M3u8VideoHandler
{
    public class DownloadSetting
    {
        public string SaveFolder { get; set; }     
        public M3u8ProcessMode M3u8ProcessMode { get; set; }
        public bool KeepM3u8File { get; set; }
        public bool KeepTsFile { get; set; }
        public bool EnableLog { get; set; } = true;
        public bool EnableDebug { get; set; } = true;
        public int ConnectLimitNum { get; set; } = 10;
    }

    public enum M3u8ProcessMode
    {
        Local,
        Remote
    }
}
