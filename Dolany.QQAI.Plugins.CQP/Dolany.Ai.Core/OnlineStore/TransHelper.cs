using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Database.Ai;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.OnlineStore
{
    public class TransHelper
    {
        public static int SellItemToShop(long QQNum, string itemName)
        {
            var price = HonorHelper.GetItemPrice(HonorHelper.Instance.FindItem(itemName), QQNum);

            var golds = OSPerson.GoldIncome(QQNum, price);
            var record = DriftItemRecord.GetRecord(QQNum);
            record.ItemConsume(itemName);
            record.Update();

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
                Price = HonorHelper.GetItemPrice(rs, 0) * 2
            }).ToArray();
        }

        public static IEnumerable<DailySellItemModel> GetDailySellItems()
        {
            var cache = GlobalVarRecord.Get("DailySellItems");
            if (!string.IsNullOrEmpty(cache.Value))
            {
                return JsonConvert.DeserializeObject<DailySellItemModel[]>(cache.Value);
            }

            var newitems = CreateDailySellItems();
            cache.Value = JsonConvert.SerializeObject(newitems);
            cache.ExpiryTime = CommonUtil.UntilTommorow();
            cache.Update();

            return newitems;
        }
    }

    public class DailySellItemModel
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }
}
