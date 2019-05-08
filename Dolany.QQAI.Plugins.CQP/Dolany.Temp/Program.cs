using System;
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
            var records = MongoService<DriftItemRecord>.Get().GroupBy(p => p.QQNum).Where(p => p.Count() >= 2);
            foreach (var record in records)
            {
                var ar = record.First();
                var br = record.Last();

                foreach (var ic in ar.ItemCount)
                {
                    var bic = br.ItemCount.FirstOrDefault(p => p.Name == ic.Name);
                    if (bic == null)
                    {
                        br.ItemCount.Add(ic);
                    }
                    else
                    {
                        bic.Count += ic.Count;
                    }
                }
                br.Update();

                MongoService<DriftItemRecord>.Delete(ar);
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
