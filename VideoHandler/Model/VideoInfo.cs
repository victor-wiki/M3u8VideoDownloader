namespace VideoHandler
{
    public class VideoInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Url2 { get; set; }

        public DownloadTaskState TaskState { get; set; }

        public int TsFileCount { get; set; }    
        
        public M3u8Info M3u8Info { get; set; }
    }

    public enum DownloadTaskState
    {
        Ready = 0,
        Running = 1,
        Finished = 2,
        Error = 3
    }
}
