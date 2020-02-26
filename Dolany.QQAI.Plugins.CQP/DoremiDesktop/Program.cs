using System;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi;
using Dolany.Ai.Doremi.Ai.Game.Xiuxian;
using Dolany.Ai.Doremi.Ai.Game.XunYuan;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;
using Dolany.Database;

namespace DoremiDesktop
{
    class Program
    {
        private static AIMgr AIMgr => AutofacSvc.Resolve<AIMgr>();

        static void Main(string[] args)
        {
            RegisterAutofac();
            AIMgr.Load(PrintMsg);

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

        private static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DataRefresher>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<Scheduler>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<RandShopper>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<ArmerMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DujieMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<LevelMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<Waiter>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<AIMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<GroupSettingMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<XunyuanCaveMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<EscapeArmerMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<BindAiMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PowerStateMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<AliveStateMgr>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DirtyFilter>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<HonorHelper>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<MongoContext>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
