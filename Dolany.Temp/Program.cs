using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.WorldLine.Doremi;
using Dolany.WorldLine.Doremi.OnlineStore;
using Dolany.Database;
using Dolany.WorldLine.Standard;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;
using Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune;
using Dolany.WorldLine.Standard.OnlineStore;
using Newtonsoft.Json;

namespace Dolany.Temp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var assemblies = new List<Assembly>()
            //{
            //    Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
            //    Assembly.GetAssembly(typeof(Program)), // DoremiDesktop
            //    Assembly.GetAssembly(typeof(IWorldLine)), // Dolany.Ai.Core
            //    Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
            //    Assembly.GetAssembly(typeof(DoremiWorldLine)), // Dolany.Ai.Doremi
            //    Assembly.GetAssembly(typeof(StandardWorldLine)) // Dolany.WorldLine.Standard
            //};
            //AutofacSvc.RegisterAutofac(assemblies);

            //var pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total") {MachineName = "."};
            //while (true)
            //{
            //    var load = pcCpuLoad.NextValue();
            //    Console.WriteLine(load);
            //    Thread.Sleep(1000);
            //}

            var i = "W8IhuvN5NRrZN23H9zYfYIO0JmQEyVZk";

            //var result = CommonUtil.PostData<object>("https://misskey.xiling.site/api/drive/folders/create", new
            //{
            //    i,
            //    name = "setu"
            //});
            //Console.WriteLine(JsonConvert.SerializeObject(result));
            var url = new Uri($"wss://misskey.xiling.site/streaming?i={i}");
            
            await Task.Run(async () =>
                           {
                               var client = new WSClient(url.ToString());
                               await client.Connect();
                               var cntBody = new
                                             {
                                                 type = "connect",
                                                 body = new
                                                        {
                                                            channel = "globalTimeline",
                                                            id      = "globalTimeline"
                                                        }
                                             };
                               await client.Send(JsonConvert.SerializeObject(cntBody));
                               var times = 1;
                               while (true)
                               {
                                   times++;
                                   await Task.Delay(1000);

                                   if (times >= 1000)
                                   {
                                       return;
                                   }
                               }
                           });

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
