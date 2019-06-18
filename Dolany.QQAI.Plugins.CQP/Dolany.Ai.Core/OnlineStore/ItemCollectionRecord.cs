using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.OnlineStore
{
    public class ItemCollectionRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, HonorItemCollection> HonorCollections { get; set; } = new Dictionary<string, HonorItemCollection>();

        public static ItemCollectionRecord Get(long QQNum)
        {
            var record = MongoService<ItemCollectionRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new ItemCollectionRecord()
            {
                QQNum = QQNum
            };
            MongoService<ItemCollectionRecord>.Insert(record);
            return record;
        }

        public List<HonorItemCollection> GetCollectionsByType(HonorType honorType)
        {
            return HonorCollections.Values.Where(p => p.Type == honorType).ToList();
        }

        public void Update()
        {
            foreach (var (_, value) in HonorCollections)
            {
                value.Update();
            }

            HonorCollections.Remove(p => p.Items.IsNullOrEmpty());
        }
    }

    public class HonorItemCollection
    {
        public string Name { get; set; }

        public HonorType Type { get; set; }

        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();

        public void Update()
        {
            Items.Remove(p => p == 0);
        }
    }

    public enum HonorType
    {
        Normal = 0,
        Limit = 1,
        Special = 2
    }
}
