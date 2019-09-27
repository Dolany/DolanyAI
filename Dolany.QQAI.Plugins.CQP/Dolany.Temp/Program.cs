using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Net;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestor = new HttpRequester();
            var html = requestor.Request("https://cn.bing.com");
            var strs = html.Split("background-image");
            var strs2 = strs[1].Split(";");
            var strs3 = strs2[0].Split(new string[] {"(", ")"}, StringSplitOptions.RemoveEmptyEntries);
            var url = strs3[1];
            var fullPath = "https://cn.bing.com" + url;

            var savePath = "./" + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
            Utility.DownloadImage(fullPath, savePath);
            var image = Image.FromFile(savePath);
            image.Save("./desktopBgImg.bmp", ImageFormat.Bmp);

            var file = new FileInfo("./desktopBgImg.bmp");
            SystemParametersInfo(20, 0, file.FullName, 0x2);

            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
        );
    }
}
