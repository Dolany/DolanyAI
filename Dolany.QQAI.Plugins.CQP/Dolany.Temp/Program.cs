using System;
using System.Collections.Generic;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var groups = MongoService<GroupSettings>.Get(p => p.BindAi != null);
            foreach (var group in groups)
            {
                if (group.BindAis == null)
                {
                    group.BindAis = new List<string>();
                }

                if (!group.BindAis.Contains(group.BindAi))
                {
                    group.BindAis.Add(group.BindAi);
                }

                group.Update();

                Console.WriteLine($"{group.Name} Completed!");
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
