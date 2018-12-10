using System;

namespace DolanyAiDesktop
{
    using Dolany.Ai.Core;

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
            UDPConnector.Send(Msg);
        }
    }
}
