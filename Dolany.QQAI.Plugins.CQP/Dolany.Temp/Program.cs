using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var members = new[] {"斯普林菲尔德太太", "子衣不待", "月牙儿", "为了帝国", 
                "一只呆雪", "Visca120", "我是叮当啊好的", "苏花半笔墨留白"};

            var rand = CommonUtil.RandSort(members);
            for (var i = 0; i < rand.Length; i++)
            {
                Console.Write(rand[i] + " ");
                if (i == 3)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
