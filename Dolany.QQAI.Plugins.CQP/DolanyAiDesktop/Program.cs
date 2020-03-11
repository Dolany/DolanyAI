using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
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
        private static GroupSettingSvc GroupSettingSvc => AutofacSvc.Resolve<GroupSettingSvc>();
        private static DataRefreshSvc DataRefreshSvc => AutofacSvc.Resolve<DataRefreshSvc>();

        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)),
                Assembly.GetAssembly(typeof(Program)),
                Assembly.GetAssembly(typeof(DbBaseEntity)),
                Assembly.GetAssembly(typeof(IWorldLine)),
                Assembly.GetAssembly(typeof(StandardWorldLine)),
                Assembly.GetAssembly(typeof(KindomStormWorldLine))
            };

            try
            {
                RegisterAutofac(assemblies);
                RegisterDataRefresher(assemblies);
                CrossWorldAiSvc.AllWorlds = GetWorlds(assemblies);

                Global.MsgPublish = PrintMsg;
                SFixedSetService.SetMaxCount("PicCache", Global.DefaultConfig.MaxPicCacheCount);
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
            var worldLine = JudgeWorldLine(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMsgReceived(MsgDTO);
        }

        private static void OnMoneyReceived(ChargeModel model)
        {
            var worldLine = JudgeWorldLine(0, model.QQNum);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMoneyReceived(model);
        }

        private static void OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            var worldLine = JudgeWorldLine(model.GroupNum, model.QQNum);
            var world = CrossWorldAiSvc.AllWorlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnGroupMemberChanged(model);
        }

        private static string JudgeWorldLine(long groupNum, long QQNum)
        {
            if (groupNum == 0)
            {
                return JudgePersonalWorldLine(QQNum);
            }

            var group = GroupSettingSvc[groupNum];
            if (group == null)
            {
                return CrossWorldAiSvc.DefaultWorldLine.Name;
            }

            return string.IsNullOrEmpty(group.WorldLine) ? CrossWorldAiSvc.DefaultWorldLine.Name : group.WorldLine;
        }

        private static string JudgePersonalWorldLine(long QQNum)
        {
            return CrossWorldAiSvc.DefaultWorldLine.Name;
        }

        private static void RegisterAutofac(List<Assembly> assemblies)
        {
            var builder = new ContainerBuilder();
            var baseType = typeof(IDependency);

            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).AsSelf()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            AutofacSvc.Container = builder.Build();
        }

        private static void RegisterDataRefresher(IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IDataMgr);
            var datamgrs = assemblies.SelectMany(p => p.GetTypes().Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract))
                .Where(type => AutofacSvc.Container.IsRegistered(type)).Select(type => AutofacSvc.Container.Resolve(type) as IDataMgr).Where(d => d != null).ToList();

            foreach (var datamgr in datamgrs)
            {
                datamgr.RefreshData();
                DataRefreshSvc.Register(datamgr);
            }
        }

        private static IWorldLine[] GetWorlds(IEnumerable<Assembly> assemblies)
        {
            var baseType = typeof(IWorldLine);
            return assemblies.SelectMany(p => p.GetTypes().Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract))
                .Where(type => AutofacSvc.Container.IsRegistered(type)).Select(type => AutofacSvc.Container.Resolve(type) as IWorldLine).Where(d => d != null)
                .ToArray();
        }
    }
}
