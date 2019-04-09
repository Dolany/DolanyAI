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
            var settings = MongoService<GroupSettings>.Get();
            var groups = new long[] {547176147};
            var selfGroups = settings.Where(p => groups.Contains(p.GroupNum));

            var days = 250;

            foreach (var selfGroup in selfGroups)
            {
                selfGroup.BindAi = "IWS2000";
                selfGroup.ExpiryTime = DateTime.Now.AddDays(days);
                selfGroup.Update();
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
