using System;
using Autofac;
using Dolany.Ai.Common;
using Dolany.Ai.Doremi;
using Dolany.Ai.Doremi.Xiuxian;

namespace DoremiDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
            RegisterAutofac();
            AIMgr.Instance.Load(PrintMsg);

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
            builder.RegisterType<DataRefresher>().AsSelf().SingleInstance();
            builder.RegisterType<Scheduler>().AsSelf().SingleInstance();
            builder.RegisterType<RandShopper>().AsSelf().SingleInstance();

            AutofacSvc.Container = builder.Build();
        }
    }
}
