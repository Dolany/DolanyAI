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
            var settings = MongoService<GroupSettings>.Get();
            foreach (var setting in settings)
            {
                setting.AuthInfo = new GroupAuthInfoModel();
                using (var db = new SqliteContext("C:/AIDB/aidb_ice.db"))
                {
                    Console.WriteLine($"Reading {setting.Name}");
                    var key = $"GroupMemberInfo-{setting.GroupNum}-";
                    var records = db.SqliteCacheModel.Where(p => p.Key.Contains(key));
                    foreach (var record in records)
                    {
                        var model = JsonConvert.DeserializeObject<AuthModel>(record.Value);
                        if (model.Role == 0)
                        {
                            setting.AuthInfo.Owner = model.QQNum;
                            Console.WriteLine($"Owner:{model.Nickname}");
                        }
                        else if (model.Role == 1)
                        {
                            setting.AuthInfo.Mgrs.Add(model.QQNum);
                            Console.WriteLine($"Mgr:{model.Nickname}");
                        }
                    }
                    Console.WriteLine();
                }
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
