using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8VideoHandler
{
    public class Downloader
    {
        private IObserver<FeedbackInfo> m_Observer;

        private Dictionary<string, VideoInfo> dictInfo = new Dictionary<string, VideoInfo>();
        private int totalFileCount = 0;
        private int downloadedFileCount = 0;
        private WebClient webClient = new WebClient();
        private const string flagFileExtension = ".m3u8";
        private string tsDownloadedLogFileName = "TsDownloaded.txt";
        private int interlockedCount = 0;
        private string mergeToolFileName = "ffmpeg.exe";
        private string localM3u8FileName = "local.m3u8";
        private string m3u8LocalKeyFileName = "key.key";
        private List<WebClient> webclients = new List<WebClient>();
        private List<Process> processes = new List<Process>();
        private List<string> toolFilePaths = new List<string>();
        private bool cancelRequired = false;
        private string saveFileExtension = ".mp4";
        private string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string SaveFolder { get; set; }

        public bool IsBusy
        {
            get { return this.dictInfo.Any(item => item.Value.TaskState == DownloadTaskState.Running); }
        }

        public DownloadSetting Setting { get; set; } = new DownloadSetting();

        public Downloader(string saveFolder)
        {
            this.SaveFolder = saveFolder;
        }

        public Downloader(string saveFolder, DownloadSetting setting)
        {
            this.SaveFolder = saveFolder;
            this.Setting = setting;
        }

        public void Subscribe(IObserver<FeedbackInfo> observer)
        {
            this.m_Observer = observer;
        }

        protected virtual void Feedback(VideoInfo info, string content, bool isDone = false, bool enableLog = true)
        {
            string fileName = LogHelper.DefaultLogFileName;

            if (info != null)
            {
                fileName = info.Name + "_" + fileName;
            }

            string logFolder = Path.Combine(this.SaveFolder, "log");

            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            string logFilePath = Path.Combine(logFolder, fileName);

            FeedbackHelper.Feedback(this.m_Observer, new FeedbackInfo() { Content = content, Source = info, IsDone = isDone }, logFilePath, enableLog);
        }

        public async Task Download(List<VideoInfo> infos)
        {
            this.totalFileCount = infos.Count;
            foreach (var info in infos)
            {
                await this.Download(info);
            }
        }

        private async Task Download(VideoInfo info)
        {
            info.TaskState = DownloadTaskState.Running;

            this.Feedback(info, "Begin to handle", false, true);

            if (!string.IsNullOrEmpty(this.SaveFolder) && !Directory.Exists(this.SaveFolder))
            {
                Directory.CreateDirectory(this.SaveFolder);
            }

            string saveFilePath = Path.Combine(this.SaveFolder, $"{info.Name}{this.saveFileExtension}");
            if (File.Exists(saveFilePath))
            {
                this.Feedback(info, $"File \"{info.Name}\" has already existed.");
                this.downloadedFileCount++;
                info.TaskState = DownloadTaskState.Finished;
                return;
            }

            string url = info.Url;

            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            string fileExtension = Path.GetExtension(fileName).ToLower();

            this.MakesureSubFolderExists(info);

            if (fileName == "m3u8" && fileExtension == "")
            {
                fileName = info.Name + ".m3u8";
                fileExtension = ".m3u8";
            }
            else if (fileName == "index" + flagFileExtension)
            {
                fileName = info.Name + flagFileExtension;
            }
            else if (fileExtension != flagFileExtension)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + flagFileExtension;
            }

            if (!this.dictInfo.ContainsKey(fileName))
            {
                this.dictInfo.Add(fileName, info);
            }

            string filePath = "";

            filePath = Path.Combine(this.GetSaveSubFolder(info), fileName);

            if (this.Setting.M3u8ProcessMode == M3u8ProcessMode.Local)
            {
                if (File.Exists(filePath))
                {
                    await this.DownloadDetails(fileName);
                    return;
                }
            }
            else
            {
                M3u8ContentParser parser = new M3u8ContentParser(url);
                M3u8Info m3u8Info = parser.Parse();
                info.M3u8Info = m3u8Info;

                var playList = m3u8Info.PlayList.Where(item => item.Duration != 0);
                info.TsFileCount = playList.Count();

                await this.ExecuteRemoteMerge(info, url);
                return;
            }

            using (WebClient client = new WebClient())
            {
                int nextIndex = Interlocked.Increment(ref interlockedCount);

                client.QueryString.Add("file", fileName);

                client.DownloadFileCompleted += Client_DownloadFileCompleted;

                this.Feedback(info, $"Begin to download file:{fileName}");

                await client.DownloadFileTaskAsync(url, filePath);
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }
            else if (e.Error != null)
            {
                this.HandleError(sender, e);
                return;
            }

            string fileName = ((WebClient)(sender)).QueryString["file"];

            this.DownloadDetails(fileName);
        }

        private void HandleError(object sender, AsyncCompletedEventArgs e)
        {
            WebClient wc = ((WebClient)(sender));
            string fileName = wc.QueryString["file"];
            string tsFileName = wc.QueryString["tsFile"];
            if (!this.dictInfo.ContainsKey(fileName))
            {
                return;
            }
            VideoInfo info = this.dictInfo[fileName];

            string errMsg = e.Error.Message;
            if (e.Error.InnerException != null)
            {
                errMsg += "," + e.Error.InnerException.Message;
            }

            info.TaskState = DownloadTaskState.Error;
            this.Feedback(info, $"Error occurs when download file \"{(string.IsNullOrEmpty(tsFileName) ? fileName : tsFileName)}\":{errMsg}");
        }

        private async Task DownloadDetails(string fileName)
        {
            if (!this.dictInfo.ContainsKey(fileName))
            {
                return;
            }

            VideoInfo info = this.dictInfo[fileName];

            this.Feedback(info, $"File \"{fileName}\" has been downloaded.");

            string fileExtension = Path.GetExtension(fileName).ToLower();

            if (fileExtension != flagFileExtension)
            {
                return;
            }

            this.Feedback(info, $"Begin to handle file");

            string filePath = Path.Combine(this.GetSaveSubFolder(info), fileName);
            string[] lines = File.ReadAllLines(filePath);

            string flagFileLine = lines.FirstOrDefault(item => item.EndsWith(flagFileExtension));
            if (!string.IsNullOrEmpty(flagFileLine))
            {
                string newFlagFileUrl = this.CombineParentUrl(this.GetUrlParentPath(info.Url), flagFileLine);

                info.Url2 = newFlagFileUrl;

                string newFileName = Path.GetFileNameWithoutExtension(fileName) + "(2)" + fileExtension;
                string newFilePath = Path.Combine(this.GetSaveSubFolder(info), newFileName);

                if (!File.Exists(newFilePath))
                {
                    this.Feedback(info, $"Download new file:{newFlagFileUrl}");

                    try
                    {
                        this.webClient.DownloadFile(newFlagFileUrl, newFilePath);
                    }
                    catch(Exception ex)
                    {
                        info.TaskState = DownloadTaskState.Error;
                        this.Feedback(info, ex.Message, false, true);
                        return;
                    }
                }

                await this.DownloadTsFiles(info, fileName, newFilePath, true);
            }
            else
            {
                await this.DownloadTsFiles(info, fileName, filePath);
            }
        }

        private string GetSaveSubFolder(VideoInfo info)
        {
            return Path.Combine(this.SaveFolder, info.Name);
        }

        private void MakesureSubFolderExists(VideoInfo info)
        {
            string subFolder = this.GetSaveSubFolder(info);
            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
            }
        }

        private string GetActualDownloadUrl(VideoInfo info)
        {
            if (!string.IsNullOrEmpty(info.Url2))
            {
                return info.Url2;
            }

            return info.Url;
        }

        private async Task DownloadTsFiles(VideoInfo info, string infoFileName, string filePath, bool isFromSubFile = false)
        {
            string subSaveFolder = this.GetSaveSubFolder(info);

            if (!Directory.Exists(subSaveFolder))
            {
                Directory.CreateDirectory(subSaveFolder);
            }

            string parentPath = this.GetUrlParentPath(this.GetActualDownloadUrl(info));
            Uri uri = new Uri(parentPath);
            string hostPrefix = uri.Scheme + "://" + uri.Host;

            M3u8ContentParser parser = new M3u8ContentParser(filePath);
            M3u8Info m3u8Info = parser.Parse();

            if (m3u8Info.Key != null)
            {
                string keyFilePath = Path.Combine(subSaveFolder, this.m3u8LocalKeyFileName);

                try
                {
                    webClient.DownloadFile(m3u8Info.Key.Url, keyFilePath);
                }
                catch (Exception ex)
                {
                    this.Feedback(info, "Failed to get key file:" + ex.Message);
                }
            }

            M3u8Info m3u8LocalInfo = new M3u8Info()
            {
                Lines = m3u8Info.Lines.Select(item => item).ToList()
            };

            int count = 0;
            var playList = m3u8Info.PlayList.Where(item => item.Duration != 0);
            info.TsFileCount = playList.Count();

            this.Feedback(info, $"There is {info.TsFileCount} ts files.");

            string[] downloadedTsFileNames = this.GetDownloadedTsFileNames(subSaveFolder);

            Dictionary<string, string> dictDownload = new Dictionary<string, string>();
            List<M3u8PlayItemInfo> localPlayList = new List<M3u8PlayItemInfo>();
            foreach (var playItem in playList)
            {
                string path = playItem.Path;
                count++;
                string url = path;
                string fileName = "";

                if (!path.StartsWith("http"))
                {
                    if (isFromSubFile || !path.StartsWith("/"))
                    {
                        url = this.CombineParentUrl(parentPath, path);
                        fileName = path;
                    }
                    else
                    {
                        url = hostPrefix + "/" + path.TrimStart('/');
                        fileName = Path.GetFileName(new Uri(url).AbsolutePath);
                    }
                }
                else
                {
                    fileName = count + ".ts";
                }

                string saveFilePath = Path.Combine(subSaveFolder, fileName);

                localPlayList.Add(new M3u8PlayItemInfo() { Index = m3u8LocalInfo.PlayList.Count(), LineIndex = playItem.LineIndex, Duration = playItem.Duration, Path = fileName });

                dictDownload.Add(url, saveFilePath);
            }

            string localM3u8FilePath = Path.Combine(subSaveFolder, this.localM3u8FileName);

            if (!File.Exists(localM3u8FilePath))
            {
                m3u8LocalInfo = M3u8Helper.UpdateContent(m3u8LocalInfo, m3u8Info.Key == null ? null : new M3u8KeyInfo() { Method = m3u8Info.Key.Method, Url = this.m3u8LocalKeyFileName }, localPlayList);
                M3u8Writer m3u8Writer = new M3u8Writer(m3u8LocalInfo);
                m3u8Writer.Write(localM3u8FilePath);
            }

            foreach (var kp in dictDownload)
            {
                string saveFilePath = kp.Value;
                string tsFileName = Path.GetFileName(kp.Value);

                if (File.Exists(saveFilePath) && downloadedTsFileNames.Contains(tsFileName))
                {
                    await this.CheckTsFiles(infoFileName, tsFileName, false);
                    continue;
                }

                if (this.cancelRequired)
                {
                    return;
                }

                using (WebClient client = new WebClient())
                {
                    this.webclients.Add(client);

                    int nextIndex = Interlocked.Increment(ref this.interlockedCount);

                    client.QueryString.Add("file", infoFileName);
                    client.QueryString.Add("tsFile", tsFileName);

                    client.DownloadFileCompleted += Client_DownloadTsFileCompleted;
                    await client.DownloadFileTaskAsync(kp.Key, saveFilePath);
                }
            }
        }

        private string[] GetDownloadedTsFileNames(string folder)
        {
            string filePath = Path.Combine(folder, this.tsDownloadedLogFileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath);
            }
            return Enumerable.Empty<string>().ToArray();
        }

        private void Client_DownloadTsFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }
            else if (e.Error != null)
            {
                this.HandleError(sender, e);
                return;
            }

            WebClient wc = (WebClient)(sender);
            string infoFileName = wc.QueryString["file"];
            string tsFileName = wc.QueryString["tsFile"];

            if (!this.dictInfo.ContainsKey(infoFileName))
            {
                return;
            }

            this.CheckTsFiles(infoFileName, tsFileName, true);
        }

        private string GetSaveFilePath(VideoInfo info)
        {
            return Path.Combine(this.SaveFolder, $"{info.Name}{this.saveFileExtension}");
        }

        public async Task CheckTsFiles(string infoFileName, string tsFileName, bool isDownloaded)
        {
            if (!this.dictInfo.ContainsKey(infoFileName))
            {
                return;
            }

            VideoInfo info = this.dictInfo[infoFileName];

            string subSaveFolder = this.GetSaveSubFolder(info);

            DirectoryInfo di = new DirectoryInfo(subSaveFolder);
            var files = di.GetFiles("*.ts").Where(item => !item.Name.StartsWith("merged_") && item.Length > 0);
            int fileCount = files.Count();

            if (isDownloaded)
            {
                string downloadedFilePath = Path.Combine(subSaveFolder, this.tsDownloadedLogFileName);

                this.Feedback(info, $"({fileCount}/{info.TsFileCount})ts文件{tsFileName}下载完成。");

                File.AppendAllLines(downloadedFilePath, new string[] { tsFileName });
            }

            if (fileCount >= info.TsFileCount && (isDownloaded || tsFileName == files.LastOrDefault()?.Name))
            {
                this.Feedback(info, $"All of ts files have been downloaded, it begins to merge.");

                string saveFilePath = this.GetSaveFilePath(info);

                await this.MergeTsFiles(info, files.OrderBy(item => item.Name.Length).ThenBy(item => item.Name).Select(item => item.FullName), saveFilePath);

                this.downloadedFileCount++;
            }
        }

        private string GetUrlParentPath(string url)
        {
            string[] items = url.Split(new char[] { '/', '\\' });

            return string.Join("/", items.Take(items.Length - 1));
        }

        private string CombineParentUrl(string url, string path)
        {
            return url.TrimEnd('/') + '/' + path.TrimStart('/');
        }

        private bool CheckMergeToolFileExists(VideoInfo info)
        {
            string exeFileName = this.mergeToolFileName;
            if (!File.Exists(exeFileName))
            {
                this.Feedback(info, $"Merge tool \"{this.mergeToolFileName}\" doesn't exist.");
                return false;
            }
            return true;
        }

        private void CopyMergeTool(string targetFilePath)
        {
            if (!File.Exists(targetFilePath))
            {
                File.Copy(this.mergeToolFileName, targetFilePath, true);
            }
        }

        public async Task MergeTsFiles(VideoInfo info, IEnumerable<string> filePaths, string saveFilePath)
        {
            if (!this.CheckMergeToolFileExists(info))
            {
                return;
            }

            string subSaveFolder = this.GetSaveSubFolder(info);

            List<string> videoPaths = new List<string>();

            string targetExeFilePath = Path.Combine(subSaveFolder, this.mergeToolFileName);
            this.CopyMergeTool(targetExeFilePath);

            if (!this.toolFilePaths.Contains(targetExeFilePath))
            {
                this.toolFilePaths.Add(targetExeFilePath);
            }

            string localM3u8FilePath = Path.Combine(subSaveFolder, this.localM3u8FileName);
            if (File.Exists(localM3u8FilePath))
            {
                await this.ExecuteMerge(targetExeFilePath, this.localM3u8FileName, Path.GetFileName(saveFilePath));
            }
            else
            {
                int count = 0;
                int maxLength = 2000;

                List<List<string>> list = new List<List<string>>();
                List<string> usedFilePaths = new List<string>();
                foreach (string filePath in filePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    count += fileName.Length;
                    usedFilePaths.Add(filePath);

                    if (count > maxLength)
                    {
                        list.Add(usedFilePaths.Select(item => item).ToList());
                        usedFilePaths.Clear();
                        count = 0;
                    }
                }

                if (list.Sum(item => item.Count()) != filePaths.Count())
                {
                    list.Add(usedFilePaths);
                }

                int i = 0;
                foreach (var item in list)
                {
                    i++;
                    string videoPath = Path.Combine(subSaveFolder, $"merged_{Path.GetFileNameWithoutExtension(saveFilePath)}{(list.Count > 1 ? ("_" + i.ToString().PadLeft(2, '0')) : "")}.ts");
                    videoPaths.Add(videoPath);

                    if (File.Exists(videoPath) && (new FileInfo(videoPath)).Length > 0)
                    {
                        continue;
                    }

                    this.Feedback(info, $"Begin to merge {item.Count} ts files to {videoPath}.");

                    await this.ExecuteLocalMerge(targetExeFilePath, item, Path.GetFileName(videoPath));

                    this.Feedback(info, $"Has been merged {item.Count} ts files to {videoPath}.");
                }

                if (list.Count > 1)
                {
                    this.Feedback(info, $"Begin to merge the {list.Count} merged ts files to {Path.GetFileName(saveFilePath)}.");

                    await this.ExecuteLocalMerge(targetExeFilePath, videoPaths, Path.GetFileName(saveFilePath));

                    this.Feedback(info, $"Has been merged {list.Count} merged ts files to {Path.GetFileName(saveFilePath)}.");
                }
            }

            string mergedVideoPath = Path.Combine(subSaveFolder, Path.GetFileName(saveFilePath));

            if (File.Exists(mergedVideoPath))
            {
                info.TaskState = DownloadTaskState.Finished;               
                info.LocalPath = saveFilePath;

                File.Move(mergedVideoPath, saveFilePath);

                this.Feedback(info, "Finished", true);

                foreach (string videoPath in videoPaths)
                {
                    if (Path.GetFileName(videoPath) != Path.GetFileName(saveFilePath))
                    {
                        File.Delete(videoPath);
                    }
                }
            }
            else
            {
                return;
            }

            this.ClearTempFiles(subSaveFolder);
        }

        private async Task ExecuteLocalMerge(string exeFilePath, IEnumerable<string> filePaths, string outputFileName)
        {
            string concatFiles = string.Join("|", filePaths.Select(item => Path.GetFileName(item)));

            string args = $"-i concat:\"{ concatFiles}\" -vcodec copy -acodec copy \"{ outputFileName}\" -y";

            await this.ExecuteCommand(exeFilePath, args);
        }

        private async Task ExecuteRemoteMerge(VideoInfo info, string m3u8Url)
        {
            if (!this.CheckMergeToolFileExists(info))
            {
                return;
            }

            string saveFilePath = this.GetSaveFilePath(info);
            string subSaveFolder = this.GetSaveSubFolder(info);
            string targetExeFilePath = Path.Combine(subSaveFolder, this.mergeToolFileName);
            string outputFilePath = Path.Combine(subSaveFolder, Path.GetFileName(saveFilePath));
            this.CopyMergeTool(targetExeFilePath);

            if (!this.toolFilePaths.Contains(targetExeFilePath))
            {
                this.toolFilePaths.Add(targetExeFilePath);
            }

            this.Feedback(info, "Begin to download...");

            await this.ExecuteMerge(targetExeFilePath, m3u8Url, Path.GetFileName(saveFilePath))
            .ContinueWith(i =>
            {
                if (File.Exists(outputFilePath))
                {
                    File.Move(outputFilePath, saveFilePath);
                }

                if (info.TaskState == DownloadTaskState.Running)
                {
                    info.TaskState = DownloadTaskState.Finished;
                    info.LocalPath = saveFilePath;

                    this.Feedback(info, "Finished", true);

                    this.ClearTempFiles(subSaveFolder);
                }
            });
        }

        private Task ExecuteMerge(string exeFilePath, string m3u8FileName, string outputFileName)
        {
            string args = $"-allowed_extensions ALL -protocol_whitelist \"file,http,crypto,tcp,https,tls\" -i \"{m3u8FileName}\" -vcodec copy -acodec copy -absf aac_adtstoasc \"{outputFileName}\"";
            return this.ExecuteCommand(exeFilePath, args);
        }

        private Task ExecuteCommand(string exeFilePath, string args)
        {
            Action exec = () =>
            {
                Process p = new Process();

                this.processes.Add(p);

                p.StartInfo.FileName = "cmd.exe";

                p.StartInfo.Arguments = args;
                p.ErrorDataReceived += P_ErrorDataReceived;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                string folder = Path.GetDirectoryName(exeFilePath);
                p.StandardInput.WriteLine(Path.GetPathRoot(folder).Trim('\\'));
                p.StandardInput.WriteLine($"cd {folder}");
                p.StandardInput.WriteLine($"{this.mergeToolFileName} {args}");

                p.BeginErrorReadLine();

                p.StandardInput.AutoFlush = true;
                p.StandardInput.Flush();
                p.StandardInput.Close();
                p.WaitForExit();
                p.Close();
                p.Dispose();
            };

            return Task.Factory.StartNew(exec);
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Process process = sender as Process;
                if (process != null)
                {
                    string args = process.StartInfo.Arguments;
                    if (!string.IsNullOrEmpty(args))
                    {
                        Regex reg = new Regex("\".*?\"", RegexOptions.IgnoreCase);
                        MatchCollection matches = reg.Matches(args);

                        string fileName = "";

                        if (matches != null && matches.Count > 0)
                        {
                            fileName = matches[matches.Count - 1].Value.Trim('"');
                        }

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            VideoInfo info = this.dictInfo.Values.FirstOrDefault(item => item.Name == Path.GetFileNameWithoutExtension(fileName));
                            if (info != null)
                            {
                                string msg = e.Data.ToLower();
                                if (msg.Contains("invalid") || msg.Contains("error"))
                                {
                                    process.Close();
                                    process.Dispose();
                                    info.TaskState = DownloadTaskState.Error;
                                    this.Feedback(info, e.Data, false, true);
                                }
                                else
                                {
                                    msg = e.Data;
                                    if (this.Setting.M3u8ProcessMode == M3u8ProcessMode.Remote && info.M3u8Info != null)
                                    {
                                        if (e.Data.Contains("Opening") && e.Data.Contains("for reading"))
                                        {
                                            reg = new Regex("\'.*?\'", RegexOptions.IgnoreCase);
                                            matches = reg.Matches(e.Data);
                                            if (matches.Count > 0)
                                            {
                                                string url = matches[0].Value.Trim('\'');
                                                string path = UrlHelper.IsValidUrl(url) ? Path.GetFileName(new Uri(url).LocalPath) : url;
                                                int index = info.M3u8Info.PlayList.Select(item => item.Path).ToList().IndexOf(path);
                                                if (index >= 0)
                                                {
                                                    msg = $"({index + 1}/{info.TsFileCount})" + msg;
                                                }
                                            }
                                        }
                                    }

                                    this.Feedback(info, msg, false, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ClearTempFiles(string folder)
        {
            try
            {
                var files = new DirectoryInfo(folder).GetFiles();
                foreach (var file in files)
                {
                    string extension = file.Extension.ToLower();
                    if (((extension == ".ts" || file.Name == this.tsDownloadedLogFileName) && Setting.KeepTsFile) || (extension == ".m3u8" && Setting.KeepM3u8File))
                    {
                        continue;
                    }
                    else
                    {
                        file.Delete();
                    }
                }
                Directory.Delete(folder);
            }
            catch (Exception e)
            {
            }
        }

        public void Cancel()
        {
            this.cancelRequired = true;

            if (this.webclients != null)
            {
                foreach (WebClient wc in this.webclients)
                {
                    if (wc != null)
                    {
                        try
                        {
                            wc.CancelAsync();
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (this.toolFilePaths.Contains(p.MainModule.FileName) ||
                    this.processes.Any(item => item != null && item.Id == p.Id))
                    {
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            this.toolFilePaths.ForEach(item =>
            {
                if (File.Exists(item) && new FileInfo(item).FullName != new FileInfo(Path.Combine(this.CurrentFolder, this.mergeToolFileName)).FullName)
                {
                    File.Delete(item);
                }
            });
        }
    }
}
