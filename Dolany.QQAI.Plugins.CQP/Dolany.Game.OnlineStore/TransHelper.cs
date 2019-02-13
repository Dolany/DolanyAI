using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;

namespace Dolany.Game.OnlineStore
{
    public class TransHelper
    {
        public static int SellItemToShop(long QQNum, string itemName)
        {
            var price = HonorHelper.Instance.GetItemPrice(HonorHelper.Instance.FindItem(itemName), QQNum);

            var golds = OSPerson.GoldIncome(QQNum, price);
            ItemHelper.Instance.ItemConsume(QQNum, itemName);

            return golds;
        }

        public static int SellHonorToShop(DriftItemRecord query, long qqNum, string honorName)
        {
            var price = HonorHelper.Instance.GetHonorPrice(honorName, qqNum);
            var golds = OSPerson.GoldIncome(qqNum, price);
            var items = HonorHelper.Instance.FindHonorItems(honorName);
            var itemsOwned = query.ItemCount.Where(ic => items.Any(i => i.Name == ic.Name)).ToList();

            foreach (var record in itemsOwned)
            {
                record.Count--;
                if (record.Count <= 0)
                {
                    query.ItemCount.Remove(record);
                }
            }

            query.HonorList.Remove(honorName);
            MongoService<DriftItemRecord>.Update(query);

            return golds;
        }

        public static DailySellItemModel[] CreateDailySellItems()
        {
            var randSort = CommonUtil.RandSort(HonorHelper.Instance.Items.ToArray()).Take(5);
            return randSort.Select(rs => new DailySellItemModel {Name = rs.Name, Price = HonorHelper.Instance.GetItemPrice(rs, 0) * 2}).ToArray();
        }

        public static DailySellItemModel[] GetDailySellItems()
        {
            const string key = "DailySellItems";
            var cache = SCacheService.Get<DailySellItemModel[]>(key);
            if (cache != null)
            {
                return cache;
            }

            var newitems = CreateDailySellItems();
            SCacheService.Cache(key, newitems);

            return newitems;
        }
    }

    public class DailySellItemModel
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }
}
