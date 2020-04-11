using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Doremi;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Database;
using Dolany.WorldLine.Standard;
using Dolany.WorldLine.Standard.Ai.Game.SignIn;
using Dolany.WorldLine.Standard.Ai.SingleCommand.Fortune;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>()
            {
                Assembly.GetAssembly(typeof(IDependency)), // Dolany.Ai.Common
                Assembly.GetAssembly(typeof(Program)), // DoremiDesktop
                Assembly.GetAssembly(typeof(IWorldLine)), // Dolany.Ai.Core
                Assembly.GetAssembly(typeof(DbBaseEntity)), // Dolany.Database
                Assembly.GetAssembly(typeof(DoremiWorldLine)), // Dolany.Ai.Doremi
                Assembly.GetAssembly(typeof(StandardWorldLine)) // Dolany.WorldLine.Standard
            };
            AutofacSvc.RegisterAutofac(assemblies);

            var osPersons = MongoService<OSPerson>.Get();
            foreach (var osPerson in osPersons)
            {
                osPerson.Level = 1;
                osPerson.GiftDic = new Dictionary<string, int>();
                osPerson.Golds = 0;
                osPerson.Diamonds = 0;
                osPerson.MaxHP = 50;
                osPerson.HonorNames = new List<string>();

                MongoService<OSPerson>.Update(osPerson);
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
