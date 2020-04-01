using System;
using System.Collections.Generic;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi;
using Dolany.Database;

namespace DoremiDesktop
{
    class Program
    {
        private static AISvc AiSvc => AutofacSvc.Resolve<AISvc>();

        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DoremiDesktop
                Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
                Assembly.GetAssembly(typeof(AISvc)) // Dolany.Ai.Doremi
            };

            AutofacSvc.RegisterAutofac(assemblies);
            AutofacSvc.RegisterDataRefresher(assemblies);

            AiSvc.Load(PrintMsg);

            var command = Console.ReadLine();
            while (command != "Exit")
            {
                command = Console.ReadLine();
            }
        }

        static void PrintMsg(string Msg)
        {
            Console.WriteLine(Msg);
        }
    }
}
