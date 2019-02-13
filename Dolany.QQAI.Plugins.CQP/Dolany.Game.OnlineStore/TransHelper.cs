using System.Linq;
using Dolany.Database;
using Dolany.Database.Ai;

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
    }
}
