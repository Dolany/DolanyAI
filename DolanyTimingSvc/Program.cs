using System;
using System.Collections.Generic;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Database;
using Dolany.WorldLine.Doremi;
using Dolany.WorldLine.KindomStorm;
using Dolany.WorldLine.Standard;

namespace DolanyTimingSvc
{
    class Program
    {
        private static TimingSvcMgr TimingSvcMgr => AutofacSvc.Resolve<TimingSvcMgr>();

        static void Main()
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DolanyTimingSvc
                Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
                Assembly.GetAssembly(typeof(IWorldLine)), // Dolany.Ai.Core
                Assembly.GetAssembly(typeof(StandardWorldLine)), // Dolany.WorldLine.Standard
                Assembly.GetAssembly(typeof(KindomStormWorldLine)), // Dolany.WorldLine.KindomStorm
                Assembly.GetAssembly(typeof(DoremiWorldLine)) // Dolany.WorldLine.Doremi
            };

            try
            {
                AutofacSvc.RegisterAutofac(assemblies);
                AutofacSvc.RegisterDataRefresher(assemblies);

                TimingSvcMgr.Init(assemblies);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetFullDetailMsg());
            }

            var command = Console.ReadLine();
            while (command != "Exit")
            {
                command = Console.ReadLine();
            }
        }
    }
}
