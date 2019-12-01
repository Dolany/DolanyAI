using System;

namespace Dolany.Ai.WSMidware
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Midware";
            Console.WriteLine("Midware Started!");
            var instance = WSMgr.Instance;

            Console.ReadKey();
        }
    }
}
