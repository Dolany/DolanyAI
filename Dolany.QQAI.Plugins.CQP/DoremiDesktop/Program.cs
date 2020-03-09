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
        private static AISvc AiSvc => AutofacSvc.Resolve<AISvc>();

        static void Main(string[] args)
        {
            RegisterAutofac();
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

        private static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DataRefreshSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<SchedulerSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<RandShopperSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<ArmerSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DujieSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<LevelSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<WaiterSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<AISvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<GroupSettingSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<XunyuanCaveSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<EscapeArmerSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<BindAiSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<PowerStateSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<AliveStateSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<DirtyFilterSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<HonorSvc>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.RegisterType<MongoContext>().AsSelf().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
