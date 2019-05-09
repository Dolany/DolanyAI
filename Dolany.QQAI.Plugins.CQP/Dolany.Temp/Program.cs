using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Core.Ai.SingleCommand.Fortune;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var record = new DailyLimitRecord()
            {
                QQNum = 123, Commands = new Dictionary<string, DailyLimitCommand>() {{"a", new DailyLimitCommand() {Times = 0, LastTime = DateTime.Now.AddDays(-1)}}}
            };
            var rr = record.Check("a", 2);
            record.Cache("a");

            rr = record.Check("a", 2);
            record.Cache("a");

            rr = record.Check("a", 2);
            record.Cache("a");

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
