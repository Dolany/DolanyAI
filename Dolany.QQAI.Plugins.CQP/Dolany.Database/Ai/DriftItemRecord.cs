using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Database.Ai
{
    public class DriftItemRecord : BaseEntity
    {
        public long QQNum { get; set; }

        public IList<DriftItemCountRecord> ItemCount { get; set; } = new List<DriftItemCountRecord>();

        public IList<string> HonorList { get; set; } = new List<string>();

        public static DriftItemRecord GetRecord(long QQNum)
        {
            var record = MongoService<DriftItemRecord>.Get(r => r.QQNum == QQNum).FirstOrDefault();
            if (record != null)
            {
                return record;
            }

            record = new DriftItemRecord();
            MongoService<DriftItemRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            ItemCount.Remove(ic => ic.Count == 0);

            MongoService<DriftItemRecord>.Update(this);
        }

        public void ItemConsume(string name, int count)
        {
            var item = ItemCount.First(ic => ic.Name == name);
            item.Count -= count;
        }
    }

    public class DriftItemCountRecord
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}
