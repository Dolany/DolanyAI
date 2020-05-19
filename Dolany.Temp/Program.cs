using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.WorldLine.Doremi;
using Dolany.WorldLine.Doremi.OnlineStore;
using Dolany.Database;
using Dolany.WorldLine.Standard;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;
using Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DoremiDesktop
                Assembly.GetAssembly(typeof(IWorldLine)), // Dolany.Ai.Core
                Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
                Assembly.GetAssembly(typeof(DoremiWorldLine)), // Dolany.Ai.Doremi
                Assembly.GetAssembly(typeof(StandardWorldLine)) // Dolany.WorldLine.Standard
            };
            AutofacSvc.RegisterAutofac(assemblies);

            var pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total") {MachineName = "."};
            while (true)
            {
                var load = pcCpuLoad.NextValue();
                Console.WriteLine(load);
                Thread.Sleep(1000);
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
