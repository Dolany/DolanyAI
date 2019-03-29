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
            var settings = MongoService<GroupSettings>.Get();
            foreach (var setting in settings)
            {
                setting.ForcedShutDown = true;
                MongoService<GroupSettings>.Update(setting);
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
