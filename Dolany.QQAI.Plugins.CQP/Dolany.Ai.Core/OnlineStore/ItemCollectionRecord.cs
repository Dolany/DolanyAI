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

        public Dictionary<string, int> AllItemsDic => HonorCollections.Select(p => p.Value).SelectMany(p => p.Items).ToDictionary(p => p.Key, p => p.Value);

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
            if (itemDic.IsNullOrEmpty())
            {
                return;
            }

            foreach (var (key, value) in itemDic)
            {
                ItemConsume(key, value);
            }
        }

        public bool CheckItem(string itemName, int count = 1)
        {
            return HonorCollections.Values.Any(p => p.Items.ContainsKey(itemName) && p.Items[itemName] >= count);
        }

        public bool CheckItem(Dictionary<string, int> itemDic)
        {
            return itemDic.All(p => CheckItem(p.Key, p.Value));
        }

        public int GetCount(string itemName)
        {
            var collection = HonorCollections.Values.FirstOrDefault(p => p.Items.ContainsKey(itemName));
            return collection?.Items[itemName] ?? 0;
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
            MongoService<ItemCollectionRecord>.Update(this);
        }

        public int AssertToGold()
        {
            var itemAssert = 0;
            foreach (var (honorName, collection) in HonorCollections)
            {
                var honorModel = HonorHelper.Instance.FindHonor(honorName);
                var honorPrice = honorModel.Items.Sum(p => p.Price) * 3 / 2;
                while (collection.Items != null && honorModel.Items.Count == collection.Items.Count)
                {
                    itemAssert += honorPrice;

                    for (var i = 0; i < collection.Items.Count; i++)
                    {
                        collection.Items[collection.Items.Keys.ElementAt(i)]--;
                    }

                    collection.Items.Remove(p => p == 0);
                }

                itemAssert += collection.Items?.Sum(p => honorModel.Items.First(item => item.Name == p.Key).Price * p.Value) ?? 0;
            }

            return itemAssert;
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
