using System;
using System.Linq;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Common.PicReview;
using Dolany.Ai.Core.SyntaxChecker;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.KindomStorm;
using Dolany.WorldLine.KindomStorm.Ai.KindomStorm;
using Dolany.WorldLine.Standard;
using Dolany.WorldLine.Standard.Ai.Game.Advanture;
using Dolany.WorldLine.Standard.Ai.Game.Gift;
using Dolany.WorldLine.Standard.Ai.Game.Lottery;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition;
using Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst;
using Dolany.WorldLine.Standard.Ai.Game.SegmentAttach;
using Dolany.WorldLine.Standard.Ai.Game.SwordExplore;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

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
            builder.RegisterType<Waiter>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PicReviewer>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DailyVipShopMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<GroupSettingMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DataRefresher>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CastleLevelMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<TownLevelMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CaveSettingHelper>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).AsSelf().SingleInstance();
            builder.RegisterType<GiftMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<LotteryMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CookingDietMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CookingLevelMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<ExpeditionSceneMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PetSkillMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PetLevelMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PetAgainstMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<SegmentMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<SEMapMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<HonorHelper>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<BindAiMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DirtyFilter>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<QQNumReflectMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<AliveStateMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CrossWorldAiMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<CommandLocker>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<SyntaxCheckerMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<Scheduler>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
