using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
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
            var query = DriftItemRecord.GetRecord(QQNum);

            var itemRecord = query.ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord != null && itemRecord.Count > 0;
        }

        public int ItemCount(long QQNum, string itemName)
        {
            var query = DriftItemRecord.GetRecord(QQNum);
            return ItemCount(query, itemName);
        }

        public int ItemCount(DriftItemRecord record, string itemName)
        {
            if (record == null || record.ItemCount.IsNullOrEmpty())
            {
                return 0;
            }

            var itemRecord = record.ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord?.Count ?? 0;
        }

        public void ItemConsume(long QQNum, string itemName, int count = 1)
        {
            var query = DriftItemRecord.GetRecord(QQNum);
            var itemRecord = query.ItemCount.First(i => i.Name == itemName);
            itemRecord.Count -= count;
            query.Update();
        }

        public (string msg, DriftItemRecord record) ItemIncome(long QQNum, string itemName, int count = 1)
        {
            var query = DriftItemRecord.GetRecord(QQNum);

            var ic = query.ItemCount.FirstOrDefault(p => p.Name == itemName);
            if (ic != null)
            {
                ic.Count += count;
            }
            else
            {
                query.ItemCount.Add(new DriftItemCountRecord {Count = count, Name = itemName});
            }

            var (s, isNewHonor) = HonorHelper.Instance.CheckHonor(query, itemName);
            if (isNewHonor)
            {
                var honorName = HonorHelper.Instance.FindHonor(itemName);
                if (query.HonorList == null)
                {
                    query.HonorList = new List<string> {honorName};
                }
                else
                {
                    query.HonorList.Add(honorName);
                }
            }

            query.Update();

            return (s, query);
        }
    }
}
