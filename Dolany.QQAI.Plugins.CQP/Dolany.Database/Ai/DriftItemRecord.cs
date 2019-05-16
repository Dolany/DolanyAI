using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Database.Ai
{
    public class DriftItemRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public IList<DriftItemCountRecord> ItemCount { get; set; } = new List<DriftItemCountRecord>();

        public IList<string> HonorList { get; set; } = new List<string>();

        public static DriftItemRecord GetRecord(long QQNum)
        {
            var record = MongoService<DriftItemRecord>.GetOnly(r => r.QQNum == QQNum);
            if (record != null)
            {
                if (record.ItemCount == null)
                {
                    record.ItemCount = new List<DriftItemCountRecord>();
                }

                if (record.HonorList == null)
                {
                    record.HonorList = new List<string>();
                }
                return record;
            }

            record = new DriftItemRecord(){QQNum = QQNum};
            MongoService<DriftItemRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            ItemCount.Remove(ic => ic.Count == 0);

            MongoService<DriftItemRecord>.Update(this);
        }

        public void ItemConsume(string name, int count = 1)
        {
            var item = ItemCount.First(ic => ic.Name == name);
            item.Count -= count;
        }

        public void ItemConsume(Dictionary<string, int> itemDic)
        {
            foreach (var (key, value) in itemDic)
            {
                ItemConsume(key, value);
            }
        }

        public int TotalItemCount()
        {
            return ItemCount.Sum(ic => ic.Count);
        }

        public int GetCount(string itemName)
        {
            if (ItemCount.IsNullOrEmpty())
            {
                return 0;
            }

            var itemRecord = ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord?.Count ?? 0;
        }

        public bool CheckItem(string itemName)
        {
            var itemRecord = ItemCount.FirstOrDefault(i => i.Name == itemName);
            return itemRecord != null && itemRecord.Count > 0;
        }
    }

    public class DriftItemCountRecord
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public override string ToString()
        {
            return $"{Name}*{Count}";
        }
    }
}
