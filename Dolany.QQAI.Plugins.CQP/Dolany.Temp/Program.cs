using System;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var itemColl = ItemCollectionRecord.Get(3325883147);
            var items = itemColl.HonorCollections.Values.SelectMany(h => h.Items.Keys);
            Console.WriteLine(items.Count());

            var allItems = HonorHelper.Instance.HonorList.SelectMany(h => h.Items).Select(p => p.Name);
            Console.WriteLine(allItems.Count());

            var des = allItems.Except(items);
            Console.WriteLine(string.Join(",", des));

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
