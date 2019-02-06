using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database.Ai;

namespace Dolany.Game.OnlineStore
{
    public class HonorHelper
    {
        public static HonorHelper Instance { get; } = new HonorHelper();

        public readonly List<DriftBottleItemModel> Items;

        private readonly Dictionary<string, DriftBottleItemModel[]> HonorDic;

        private readonly int SumRate;

        private HonorHelper()
        {
            HonorDic = CommonUtil.ReadJsonData<Dictionary<string, DriftBottleItemModel[]>>("driftBottleItemData");
            Items = HonorDic.SelectMany(p =>
            {
                foreach (var model in p.Value)
                {
                    model.Honor = p.Key;
                }

                return p.Value;
            }).ToList();

            SumRate = Items.Sum(p => p.Rate);
        }

        public DriftBottleItemModel FindItem(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name);
        }

        public DriftBottleItemModel[] FindHonorItems(string name)
        {
            return HonorDic.Keys.Contains(name) ? HonorDic[name] : null;
        }

        public int GetItemPrice(DriftBottleItemModel item)
        {
            var rate = Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2);
            return (int) (100 / rate);
        }

        public string FindHonor(string itemName)
        {
            var item = FindItem(itemName);
            return item == null ? string.Empty : item.Honor;
        }

        public (string msg, bool IsNewHonor) CheckHonor(DriftItemRecord record, string itemName)
        {
            if (record == null || record.ItemCount == null)
            {
                return (string.Empty, false);
            }

            var honorName = FindHonor(itemName);
            var honorCount = 0;
            if (record.HonorList == null || !record.HonorList.Contains(honorName))
            {
                honorCount = record.ItemCount.Count(p => this.HonorDic[honorName].Any(ps => ps.Name == p.Name));
            }

            if (honorCount == 0)
            {
                return (string.Empty, false);
            }

            var honorItems = FindHonorItems(honorName);
            if (honorCount < honorItems.Length)
            {
                return ($"成就 {honorName} 完成度：{honorCount}/{honorItems.Length}", false);
            }

            return ($"恭喜你解锁了成就 {honorName}! (集齐物品：{string.Join("，", honorItems.Select(p => p.Name))})", true);
        }

        private DriftBottleItemModel LocalateItem(int index)
        {
            var totalSum = 0;
            foreach (var item in this.Items)
            {
                if (index < totalSum + item.Rate)
                {
                    return item;
                }

                totalSum += item.Rate;
            }

            return this.Items.First();
        }

        public DriftBottleItemModel RandItem()
        {
            return LocalateItem(CommonUtil.RandInt(this.SumRate));
        }

        public double ItemRate(DriftBottleItemModel item)
        {
            return Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2);
        }
    }

    public class DriftBottleItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

        public string Honor { get; set; }
    }
}
