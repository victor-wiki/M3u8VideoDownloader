using System;
using System.IO;

namespace VideoHandler
{
    public class LogHelper
    {
        public static string DefaultLogFileName { get; set; } = "log.txt";
        public static bool EnableDebug { get; set; }
        private static object obj = new object();

        public static void LogInfo(string filePath, string info)
        {
            string content = $"{DateTime.Now.ToString("yyyyMMdd HH.mm.ss")}:{info}";

            lock(obj)
            {
                File.AppendAllLines(filePath, new string[] { content });
            }           

            if (EnableDebug)
            {
                Console.WriteLine(info);
            }            
        }
    }
}
