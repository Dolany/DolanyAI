using System;
using System.Threading;
using Dolany.Ai.Common;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 50; i++)
            {
                Console.WriteLine(Rander.RandInt(2));
                Thread.Sleep(50);
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
