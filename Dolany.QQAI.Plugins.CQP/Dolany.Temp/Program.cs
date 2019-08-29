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
            var itemColl = ItemCollectionRecord.Get(1304399144);
            var items = itemColl.HonorCollections.Values.SelectMany(h => h.Items.Keys);
            Console.WriteLine(items.Count());

            var allItems = HonorHelper.Instance.HonorList.SelectMany(h => h.Items);
            Console.WriteLine(allItems.Count());

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
