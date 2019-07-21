using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.OnlineStore
{
    public class TransHelper
    {
        public static int SellItemToShop(long QQNum, string itemName)
        {
            var price = HonorHelper.GetItemPrice(HonorHelper.Instance.FindItem(itemName), QQNum);

            var golds = OSPerson.GoldIncome(QQNum, price);
            var record = ItemCollectionRecord.Get(QQNum);
            record.ItemConsume(itemName);
            record.Update();

            return golds;
        }

        public static int SellHonorToShop(ItemCollectionRecord record, long qqNum, string honorName)
        {
            var price = HonorHelper.Instance.GetHonorPrice(honorName, qqNum);
            var golds = OSPerson.GoldIncome(qqNum, price);
            var honorCollection = record.HonorCollections[honorName];
            for (var i = 0; i < honorCollection.Items.Count; i++)
            {
                var (key, value) = honorCollection.Items.ElementAt(i);
                honorCollection.Items[key] = value - 1;
            }
            record.Update();

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
                Price = rs.Price * 2,
                Attr = string.Join(",", rs.Attributes)
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

        public string Attr { get; set; }
    }
}
