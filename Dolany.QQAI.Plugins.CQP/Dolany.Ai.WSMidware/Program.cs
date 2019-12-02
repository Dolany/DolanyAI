using System;
using System.Threading;

namespace Dolany.Ai.WSMidware
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Global.Config.ConsoleName;
            Console.WriteLine("Midware Started!");
            var instance = WSMgr.Instance;

            while (Console.ReadLine() != "Exit")
            {
                Thread.Sleep(1000);
            }
        }
    }
}
