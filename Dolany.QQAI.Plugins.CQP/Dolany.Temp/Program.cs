using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var record = MongoService<DriftItemRecord>.Get(p => p.QQNum == 601844608).First();
            record.ItemCount = CommonUtil.RandSort(record.ItemCount.ToArray());
            record.Update();

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
