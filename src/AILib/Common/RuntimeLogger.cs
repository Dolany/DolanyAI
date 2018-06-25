using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AILib
{
    public static class RuntimeLogger
    {
        private static string LogPath = "./RuntimeLog/";

        public static void Log(string log)
        {
            var steam = CheckFile();
            byte[] data = new UTF8Encoding().GetBytes(log + "\r");
            steam.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            steam.Flush();
            steam.Close();
        }

        private static FileStream CheckFile()
        {
            FileInfo fi = new FileInfo(LogPath + DateTime.Now.ToString("yyyyMMdd") + ".log");
            if (!fi.Exists)
            {
                return fi.Create();
            }

            return fi.Open(FileMode.Open);
        }
    }
}