using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Common
{
    using System.IO;

    public static class RuntimeLogger
    {
        private const string LogPath = "./RuntimeLog/";
        private static readonly object lockObj = new object();
        public static int ErrorCount;

        public static void Log(string log)
        {
            lock (lockObj)
            {
                var steam = CheckFile();
                var data = new UTF8Encoding().GetBytes($"{DateTime.Now.ToCommonString()}:{log}\r\n");
                steam.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                steam.Flush();
                steam.Close();
            }
        }

        public static void Log(Exception ex)
        {
            ErrorCount++;
            while (true)
            {
                Log(ex.Message + "\r\n" + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    continue;
                }

                break;
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
            return !fi.Exists ? fi.Create() : fi.Open(FileMode.Append, FileAccess.Write);
        }
    }
}
