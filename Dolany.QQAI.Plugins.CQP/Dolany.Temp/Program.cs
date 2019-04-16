using System;
using Dolany.Ai.Core.Ai.SingleCommand.Fortune;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var requestor = new FortuneRequestor(new MsgInformationEx() {Msg = "白羊"}, ((msg, s) => { }));
            requestor.Work();

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }

    public class AuthModel
    {
        public long QQNum { get;set; }

        public long GroupNum { get; set; }

        public int Role { get; set; }

        public string Nickname { get; set; }
    }
}
