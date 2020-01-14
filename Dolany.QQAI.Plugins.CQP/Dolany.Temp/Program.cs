using System;
using System.Runtime.InteropServices;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            

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
