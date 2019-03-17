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
            var limitDic = CommonUtil.ReadJsonData<Dictionary<string, DriftBottleItemModel[]>>("driftBottleItemData_Limit");
            foreach (var bim in limitDic)
            {
                var (key, value) = bim;
                HonorDic.Add(key, value);
            }
            Items = HonorDic.SelectMany(p =>
            {
                var (key, value) = p;
                foreach (var model in value)
                {
                    model.Honor = key;
                }

                return value;
            }).ToList();

            SumRate = Items.Sum(p => p.Rate); 
            LimitItems = HonorDic.Where(p => p.Key.Contains("限定"))
                .SelectMany(p => p.Value.Select(item => item.Name)).ToList();
        }

        public List<string> LimitItems { get; set; }

        public DriftBottleItemModel FindItem(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name);
        }

        public DriftBottleItemModel[] FindHonorItems(string name)
        {
            return HonorDic.Keys.Contains(name) ? HonorDic[name] : null;
        }

        public int GetItemPrice(DriftBottleItemModel item, long qqNum)
        {
            if (item.Rate == 0)
            {
                return 1;
            }

            var rate = Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2);
            var price = (int) (100 / rate);
            var osPerson = OSPerson.GetPerson(qqNum);
            if (osPerson.CheckBuff("疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public int GetHonorPrice(string honorName, long qqNum)
        {
            var items = FindHonorItems(honorName);
            var price = items.Sum(item =>
            {
                if (item.Rate == 0)
                {
                    return 1;
                }

                var rate = Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2);
                return (int) (100 / rate);
            }) * 3 / 2;

            var osPerson = OSPerson.GetPerson(qqNum);
            if (osPerson.CheckBuff("疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public string FindHonor(string itemName)
        {
            var item = FindItem(itemName);
            return item == null ? string.Empty : item.Honor;
        }

        public (string msg, bool IsNewHonor) CheckHonor(DriftItemRecord record, string itemName)
        {
            if (record?.ItemCount == null)
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
            if (item.Rate == 0)
            {
                return 0;
            }
            return Math.Round(item.Rate * 1.0 / this.SumRate * 100, 2);
        }

        public IList<string> GetOrderedItemsStr(IEnumerable<DriftItemCountRecord> items)
        {
            var itemHonorDic = items.Select(i => new {Honor = FindHonor(i.Name), i.Name, i.Count})
                .GroupBy(p => p.Honor)
                .ToDictionary(p => p.Key, p => p.ToList());
            var list = itemHonorDic.Select(kv => $"{kv.Key}:{string.Join(",", kv.Value.Select(p => $"{p.Name}({p.Count})"))}");
            return list.ToList();
        }
    }

    public class DriftBottleItemModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }

        public string Honor { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
