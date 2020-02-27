﻿using System;
using System.Linq;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.KindomStorm;
using Dolany.WorldLine.Standard;

namespace DolanyAiDesktop
{
    class Program
    {
        private static readonly IWorldLine[] worlds = {new StandardWorldLine(), new KindomStormWorldLine(),};
        private static IWorldLine DefaultWorldLine => worlds.First(w => w.IsDefault);
        private static DateTime LastPrintTime = DateTime.Now;

        private static Waiter Waiter => AutofacSvc.Resolve<Waiter>();
        private static CrossWorldAiMgr CrossWorldAiMgr => AutofacSvc.Resolve<CrossWorldAiMgr>();

        static void Main(string[] args)
        {
            RegisterAutofac();

            try
            {
                Global.MsgPublish = PrintMsg;
                SFixedSetService.SetMaxCount("PicCache", Global.DefaultConfig.MaxPicCacheCount);
                Waiter.MsgReceivedCallBack = OnMsgReceived;
                Waiter.MoneyReceivedCallBack = OnMoneyReceived;
                Waiter.GroupMemberChangeCallBack = OnGroupMemberChanged;

                AIAnalyzer.Sys_StartTime = DateTime.Now;
                CrossWorldAiMgr.AllWorlds = worlds;

                foreach (var worldLine in worlds)
                {
                    worldLine.AIGroup.AddRange(CrossWorldAiMgr.CrossWorldAis.ToArray());
                    worldLine.Load();
                }

                Waiter.Listen();
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
            if (LastPrintTime.AddMinutes(2) < DateTime.Now)
            {
                LastPrintTime = DateTime.Now;
                Console.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
            }
            Console.WriteLine(Msg);
        }

        private static void OnMsgReceived(MsgInformation MsgDTO)
        {
            var worldLine = JudgeWorldLine(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var world = worlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMsgReceived(MsgDTO);
        }

        private static void OnMoneyReceived(ChargeModel model)
        {
            var worldLine = JudgeWorldLine(0, model.QQNum);
            var world = worlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMoneyReceived(model);
        }

        private static void OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            var worldLine = JudgeWorldLine(model.GroupNum, model.QQNum);
            var world = worlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnGroupMemberChanged(model);
        }

        private static string JudgeWorldLine(long groupNum, long QQNum)
        {
            if (groupNum == 0)
            {
                return JudgePersonalWorldLine(QQNum);
            }

            var group = AutofacSvc.Resolve<GroupSettingMgr>()[groupNum];
            if (group == null)
            {
                return DefaultWorldLine.Name;
            }

            return string.IsNullOrEmpty(group.WorldLine) ? DefaultWorldLine.Name : group.WorldLine;
        }

        private static string JudgePersonalWorldLine(long QQNum)
        {
            return DefaultWorldLine.Name;
        }

        private static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            var baseType = typeof(IDependency);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            builder.RegisterAssemblyTypes(assemblies).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).AsSelf()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
