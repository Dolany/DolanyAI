using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class RuntimeLogger
    {
        private static string LogPath = "./RuntimeLog/";
        private static readonly object lockObj = new object();

        /// <summary>
        ///
        /// </summary>
        /// <param name="log"></param>
        public static void Log(string log)
        {
            lock (lockObj)
            {
                var steam = CheckFile();
                byte[] data = new UTF8Encoding().GetBytes($"{DateTime.Now.ToString()}:{log}\r");
                steam.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                steam.Flush();
                steam.Close();
            }
        }

        private static FileStream CheckFile()
        {
            DirectoryInfo dir = new DirectoryInfo(LogPath);
            if (!dir.Exists)
            {
                dir.Create();
            }

            FileInfo fi = new FileInfo(LogPath + DateTime.Now.ToString("yyyyMMdd") + ".log");
            if (!fi.Exists)
            {
                return fi.Create();
            }

            return fi.Open(FileMode.Append, FileAccess.Write);
        }
    }
}