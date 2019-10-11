using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Net;
using Dolany.Ai.Core.OnlineStore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var allItems = HonorHelper.Instance.HonorList.Where(h => !(h is LimitHonorModel)).SelectMany(h => h.Items);
            var price = allItems.Sum(item => item.Price) * 3 / 2;

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
