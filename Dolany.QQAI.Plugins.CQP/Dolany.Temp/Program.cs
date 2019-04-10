using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Newtonsoft.Json;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var records = MongoService<DriftItemRecord>.Get(p => p.HonorList != null && p.HonorList.Contains("镀金骑士(201903限定)"));
            foreach (var record in records)
            {
                record.HonorList.Remove("镀金骑士(201903限定)");
                record.HonorList.Add("镀金骑士");

                record.Update();
            }

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
