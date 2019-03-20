using System;
using System.Linq;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var limitRecords = MongoService<DriftItemRecord>.Get(p => p.HonorList.Any(h => h.Contains("限定")));
            foreach (var record in limitRecords)
            {
                var honor = record.HonorList.First(h => h.Contains("限定"));
                var strs = honor.Split(new char[]{'(', ')'});
                record.HonorList.Remove(honor);
                record.HonorList.Add(strs[0]);

                record.Update();
            }

            Console.ReadKey();
        }
    }
}
