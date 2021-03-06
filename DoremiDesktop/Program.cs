﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Doremi;
using Dolany.Database;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.KindomStorm;
using Dolany.WorldLine.Standard;

namespace DoremiDesktop
{
    class Program
    {
        private static WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();
        private static CrossWorldAiSvc CrossWorldAiSvc => AutofacSvc.Resolve<CrossWorldAiSvc>();

        static void Main()
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DoremiDesktop
                Assembly.GetAssembly(typeof(IWorldLine)), // Dolany.Ai.Core
                Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
                Assembly.GetAssembly(typeof(DoremiWorldLine)), // Dolany.Ai.Doremi
                Assembly.GetAssembly(typeof(StandardWorldLine)), // Dolany.WorldLine.Standard
                Assembly.GetAssembly(typeof(KindomStormWorldLine)) // Dolany.WorldLine.KindomStorm
            };

            try
            {
                AutofacSvc.RegisterAutofac(assemblies);
                AutofacSvc.RegisterDataRefresher(assemblies);

                Global.MsgPublish = PrintMsg;
                SFixedSetService.SetMaxCount("PicCache_Doremi", 200);
                AIAnalyzer.Sys_StartTime = DateTime.Now;

                CrossWorldAiSvc.InitWorlds(assemblies);
                CrossWorldAiSvc.DefaultWorldLine = CrossWorldAiSvc["Doremi"];

                WaiterSvc.Listen();
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

        private static void PrintMsg(string Msg)
        {
            Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} {Msg}");
        }
    }
}
