using System;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var oldRecords = MongoService<DriftItemRecord>.Get();
            Console.WriteLine("Total Counts: " + oldRecords.Count);
            var c = 0;
            foreach (var oldRecord in oldRecords)
            {
                var newRecord = ItemCollectionRecord.Get(oldRecord.QQNum);
                foreach (var countRecord in oldRecord.ItemCount)
                {
                    var honorName = HonorHelper.Instance.FindHonorName(countRecord.Name);
                    var honor = HonorHelper.Instance.FindHonor(honorName);

                    if (!newRecord.HonorCollections.ContainsKey(honorName))
                    {
                        newRecord.HonorCollections.Add(honorName, new HonorItemCollection()
                        {
                            Name = honorName,
                            Type = honor is LimitHonorModel ? HonorType.Limit : HonorType.Normal
                        });
                    }

                    var collection = newRecord.HonorCollections[honorName];
                    if (!collection.Items.ContainsKey(countRecord.Name))
                    {
                        collection.Items.Add(countRecord.Name, 0);
                    }

                    collection.Items[countRecord.Name] = countRecord.Count;
                }

                c++;
                if (c % 100 == 0)
                {
                    Console.WriteLine($"{c} completed.");
                }
                newRecord.Update();
            }

            Console.WriteLine("Completed");
            Console.ReadKey();
        }
    }
}
