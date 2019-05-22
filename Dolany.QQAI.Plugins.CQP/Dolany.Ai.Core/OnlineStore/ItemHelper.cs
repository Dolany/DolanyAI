using System.Linq;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.OnlineStore
{
    public static class ItemHelper
    {
        public static string ItemIncome(this DriftItemRecord record, string itemName, int count = 1)
        {
            var ic = record.ItemCount.FirstOrDefault(p => p.Name == itemName);
            if (ic != null)
            {
                ic.Count += count;
            }
            else
            {
                record.ItemCount.Add(new DriftItemCountRecord {Count = count, Name = itemName});
            }

            var honorName = HonorHelper.Instance.FindHonorName(itemName);
            var isNewHonor = HonorHelper.Instance.CheckHonor(record, honorName, out var msg);
            if (isNewHonor)
            {
                record.HonorList.Add(honorName);
            }

            record.Update();

            return msg;
        }
    }
}
