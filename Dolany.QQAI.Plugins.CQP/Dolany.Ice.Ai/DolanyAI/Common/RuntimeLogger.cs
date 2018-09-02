using System.IO;
using System;
using System.Text;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class RuntimeLogger
    {
        private const string LogPath = "./RuntimeLog/";
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

        public static void Log(Exception ex)
        {
            while (true)
            {
                Log(ex.Message + '\r' + ex.StackTrace);
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