using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Game.OnlineStore
{
    public class ItemHelper
    {
        public static ItemHelper Instance { get; } = new ItemHelper();

        private ItemHelper()
        {

        }

        public bool CheckItem(long QQNum, string itemName)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == QQNum).FirstOrDefault();
            if (query == null || query.ItemCount.IsNullOrEmpty())
            {
                return false;
            }

            var itemRecord = query.ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord != null && itemRecord.Count > 0;
        }

        public int ItemCount(long QQNum, string itemName)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == QQNum).FirstOrDefault();
            return ItemCount(query, itemName);
        }

        public int ItemCount(DriftItemRecord record, string itemName)
        {
            if (record == null || record.ItemCount.IsNullOrEmpty())
            {
                return 0;
            }

            var itemRecord = record.ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord == null ? 0 : itemRecord.Count;
        }

        public void ItemConsume(long QQNum, string itemName, int count = 1)
        {
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == QQNum).First();
            var itemRecord = query.ItemCount.First(i => i.Name == itemName);
            itemRecord.Count -= count;
            if (itemRecord.Count <= 0)
            {
                query.ItemCount.Remove(itemRecord);
            }

            MongoService<DriftItemRecord>.Update(query);
        }

        public (string msg, DriftItemRecord record) ItemIncome(long QQNum, string itemName, int count = 1)
        {
            string msg;
            var query = MongoService<DriftItemRecord>.Get(r => r.QQNum == QQNum).FirstOrDefault();
            if (query == null)
            {
                query = new DriftItemRecord()
                {
                    ItemCount = new List<DriftItemCountRecord>() { new DriftItemCountRecord
                    {
                        Count = count,
                        Name = itemName
                    }}
                };

                var (s, _) = HonorHelper.Instance.CheckHonor(query, itemName);
                msg = s;
                MongoService<DriftItemRecord>.Insert(query);
            }
            else
            {
                var ic = query.ItemCount.FirstOrDefault(p => p.Name == itemName);
                if (ic != null)
                {
                    ic.Count += count;
                }
                else
                {
                    query.ItemCount.Add(new DriftItemCountRecord
                    {
                        Count = count,
                        Name = itemName
                    });
                }

                var (s, isNewHonor) = HonorHelper.Instance.CheckHonor(query, itemName);
                msg = s;
                if (isNewHonor)
                {
                    var honorName = HonorHelper.Instance.FindHonor(itemName);
                    if (query.HonorList == null)
                    {
                        query.HonorList = new List<string>() {honorName};
                    }
                    else
                    {
                        query.HonorList.Add(honorName);
                    }
                }
                MongoService<DriftItemRecord>.Update(query);
            }

            return (msg, query);
        }
    }
}
