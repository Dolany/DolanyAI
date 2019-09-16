using System;
using Dolany.Ai.Core.Common;
using Newtonsoft.Json;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = GroupSettingMgr.Instance.SettingDic;
            Console.WriteLine(JsonConvert.SerializeObject(settings));

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
