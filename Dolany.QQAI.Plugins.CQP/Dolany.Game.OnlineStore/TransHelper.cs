using System.Linq;
using Dolany.Ai.Common;
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
            var items = HonorHelper.Instance.FindHonor(honorName).Items;
            var itemsOwned = query.ItemCount.Where(ic => items.Any(i => i.Name == ic.Name)).ToList();

            foreach (var record in itemsOwned)
            {
                record.Count--;
            }

            query.HonorList.Remove(honorName);
            query.Update();

            return golds;
        }

        private static DailySellItemModel[] CreateDailySellItems()
        {
            var honors = HonorHelper.Instance.HonorList.Where(h => !(h is LimitHonorModel));
            var items = honors.SelectMany(h => h.Items).ToArray();
            var randSort = CommonUtil.RandSort(items).Take(5);
            return randSort.Select(rs => new DailySellItemModel
            {
                Name = rs.Name,
                Price = HonorHelper.Instance.GetItemPrice(rs, 0) * 2
            }).ToArray();
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
