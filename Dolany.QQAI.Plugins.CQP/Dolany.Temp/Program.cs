using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var honorList = HonorHelper.Instance.HonorList;
            var attrs = new[] {"钢铁", "海洋", "深渊", "自然", "神秘"};
            foreach (var attr in attrs)
            {
                var sumItem = honorList.Sum(h => h.Items.Count(item => item.Attributes != null && item.Attributes.Contains(attr)));
                var sumPrice = honorList.Sum(h => h.Items.Where(item => item.Attributes != null && item.Attributes.Contains(attr)).Sum(p => p.Price));

                Console.WriteLine($"{attr}:  sumItem:{sumItem},sumPrice:{sumPrice}");
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
