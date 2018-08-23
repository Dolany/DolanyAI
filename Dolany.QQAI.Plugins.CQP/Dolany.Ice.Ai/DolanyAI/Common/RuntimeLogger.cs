using System.IO;
using System;
using System.Text;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class RuntimeLogger
    {
        private static string LogPath = "./RuntimeLog/";
        private static readonly object lockObj = new object();

        public static void Log(string log)
        {
            lock (lockObj)
            {
                var steam = CheckFile();
                var data = new UTF8Encoding().GetBytes($"{DateTime.Now.ToCommonString()}:{log}\r");
                steam.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                steam.Flush();
                steam.Close();
            }
        }

        private static FileStream CheckFile()
        {
            var dir = new DirectoryInfo(LogPath);
            if (!dir.Exists)
            {
                dir.Create();
            }

            var fi = new FileInfo(LogPath + DateTime.Now.ToString("yyyyMMdd") + ".log");
            if (!fi.Exists)
            {
                return fi.Create();
            }

            return fi.Open(FileMode.Append, FileAccess.Write);
        }
    }
}