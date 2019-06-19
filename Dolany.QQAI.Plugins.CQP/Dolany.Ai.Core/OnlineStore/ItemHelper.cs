using System.Linq;

namespace Dolany.Ai.Core.OnlineStore
{
    public static class ItemHelper
    {
        public static string ItemIncome(this ItemCollectionRecord record, string itemName, int count = 1)
        {
            var isNew = false;
            var honorName = HonorHelper.Instance.FindHonorName(itemName);
            var honor = HonorHelper.Instance.FindHonor(honorName);
            if (!record.HonorCollections.ContainsKey(honorName))
            {
                record.HonorCollections.Add(honorName, new HonorItemCollection()
                {
                    Name = honorName,
                    Type = honor is LimitHonorModel ? HonorType.Limit : HonorType.Normal
                });
                isNew = true;
            }

            var collection = record.HonorCollections[honorName];
            if (!collection.Items.ContainsKey(itemName))
            {
                collection.Items.Add(itemName, 0);
                isNew = true;
            }

            collection.Items[itemName] += count;
            record.Update();

            if (!isNew || collection.Items.Count < honor.Items.Count)
            {
                return $"成就 {honor.FullName} 完成度：{collection.Items.Count}/{honor.Items.Count}";
            }

            return $"恭喜你解锁了成就 {honor.FullName}! (集齐物品：{string.Join("，", honor.Items.Select(p => p.Name))})";
        }
    }
}
