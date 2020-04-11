using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Doremi;
using Dolany.Database;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.KindomStorm;
using Dolany.WorldLine.Standard;

namespace DolanyAiDesktop
{
    class Program
    {
        private static WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();
        private static CrossWorldAiSvc CrossWorldAiSvc => AutofacSvc.Resolve<CrossWorldAiSvc>();

        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DolanyAiDesktop
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
                CrossWorldAiSvc.InitWorlds(assemblies);
                CrossWorldAiSvc.DefaultWorldLine = CrossWorldAiSvc["经典"];

                Global.MsgPublish = PrintMsg;
                SFixedSetService.SetMaxCount("PicCache", 200);
                WaiterSvc.MsgReceivedCallBack = OnMsgReceived;
                WaiterSvc.MoneyReceivedCallBack = OnMoneyReceived;
                WaiterSvc.GroupMemberChangeCallBack = OnGroupMemberChanged;

                AIAnalyzer.Sys_StartTime = DateTime.Now;

                foreach (var worldLine in CrossWorldAiSvc.AllWorlds)
                {
                    worldLine.Init();
                    worldLine.AIGroup.AddRange(CrossWorldAiSvc.CrossWorldAis);
                    worldLine.Load();
                }

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

        private static void OnMsgReceived(MsgInformation MsgDTO)
        {
            var worldLine = CrossWorldAiSvc.JudgeWorldLine(MsgDTO.FromGroup);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMsgReceived(MsgDTO);
        }

        private static void OnMoneyReceived(ChargeModel model)
        {
            var worldLine = CrossWorldAiSvc.JudgeWorldLine(0);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMoneyReceived(model);
        }

        private static void OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            var worldLine = CrossWorldAiSvc.JudgeWorldLine(model.GroupNum);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnGroupMemberChanged(model);
        }
    }
}
