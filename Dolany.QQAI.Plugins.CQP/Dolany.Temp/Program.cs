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
            //using (var mainDb = new SqliteContext(Configger.Instance["mainDb"]))
            //{
            //    var mm = mainDb.SqliteCacheModel;
            //    using (var secondaryDb = new SqliteContext(Configger.Instance["secondaryDb"]))
            //    {
            //        var 
            //    }
            //}

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
