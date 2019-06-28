using System;
using Dolany.Ai.Doremi;

namespace DoremiDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
            AIMgr.Instance.Load(PrintMsg);

            var command = Console.ReadLine();
            while (command != "Exit")
            {
                command = Console.ReadLine();
            }
        }

        static void PrintMsg(string Msg)
        {
            Console.WriteLine(Msg);
        }
    }
}
