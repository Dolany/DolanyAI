using System;
using System.Threading;
using Autofac;
using Dolany.Ai.Common;

namespace Dolany.Ai.WSMidware
{
    class Program
    {
        private static WSMgr WSMgr => AutofacSvc.Resolve<WSMgr>();

        static void Main(string[] args)
        {
            RegisterAutofac();

            Console.Title = Global.Config.ConsoleName;
            Console.WriteLine("Midware Started!");

            WSMgr.Init();

            while (Console.ReadLine() != "Exit")
            {
                Thread.Sleep(1000);
            }
        }

        private static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DataRefreshSvc>().AsSelf().SingleInstance();
            builder.RegisterType<SchedulerSvc>().AsSelf().SingleInstance();
            builder.RegisterType<WSMgr>().AsSelf().SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
