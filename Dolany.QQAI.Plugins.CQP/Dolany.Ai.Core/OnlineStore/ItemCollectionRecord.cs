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

        public List<string> HonorList =>
            HonorCollections.Where(colle => colle.Value.Items.Count == HonorHelper.Instance.FindHonor(colle.Key).Items.Count).Select(p => p.Key).ToList();

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

        public void ItemConsume(string name, int count = 1)
        {
            var collection = HonorCollections.First(p => p.Value.Items.Any(i => i.Key == name));
            collection.Value.Items[name] -= count;
        }

        public void ItemConsume(Dictionary<string, int> itemDic)
        {
            foreach (var (key, value) in itemDic)
            {
                ItemConsume(key, value);
            }
        }

        public bool CheckItem(string itemName)
        {
            return HonorCollections.Values.Any(p => p.Items.ContainsKey(itemName));
        }

        public int GetCount(string itemName)
        {
            var collection = HonorCollections.Values.FirstOrDefault(p => p.Items.ContainsKey(itemName));
            return collection == null ? 0 : collection.Items[itemName];
        }

        public int TotalItemCount()
        {
            return HonorCollections.Values.Sum(p => p.Items.Sum(i => i.Value));
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
