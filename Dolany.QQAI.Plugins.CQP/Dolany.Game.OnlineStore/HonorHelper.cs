using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database.Ai;

namespace Dolany.Game.OnlineStore
{
    public class HonorHelper
    {
        public static HonorHelper Instance { get; set; } = new HonorHelper();

        public List<DriftBottleItemModel> Items { get; set; }

        public List<DriftBottleLimitItemModel> LimitItems { get; set; }

        private Dictionary<string, DriftBottleItemModel[]> HonorDic;

        private Dictionary<string, DriftBottleLimitItemModel[]> LimitHonorDic;

        private int SumRate;

        private HonorHelper()
        {
            Refresh();
        }

        public void Refresh()
        {
            HonorDic = CommonUtil.ReadJsonData<Dictionary<string, DriftBottleItemModel[]>>("driftBottleItemData");
            LimitHonorDic = CommonUtil.ReadJsonData<Dictionary<string, DriftBottleLimitItemModel[]>>("driftBottleItemData_Limit");

            Items = HonorDic.SelectMany(p =>
            {
                var (key, value) = p;
                foreach (var model in value)
                {
                    model.Honor = key;
                }

                return value;
            }).ToList();

            LimitItems = LimitHonorDic.SelectMany(p =>
            {
                var (key, value) = p;
                foreach (var model in value)
                {
                    model.Honor = key;
                }

                return value;
            }).ToList();

            SumRate = Items.Sum(p => p.Rate) + LimitItems.Sum(p => p.Rate); 
        }

        public DriftBottleItemModel FindItem(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name) ?? LimitItems.FirstOrDefault(li => li.Name == name);
        }

        public DriftBottleItemModel[] FindHonorItems(string name)
        {
            if (HonorDic.ContainsKey(name))
            {
                return HonorDic[name];
            }

            return LimitHonorDic.ContainsKey(name) ? LimitHonorDic[name] : null;
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
                if (HonorDic.ContainsKey(honorName))
                {
                    honorCount = record.ItemCount.Count(p => this.HonorDic[honorName].Any(ps => ps.Name == p.Name));
                }
                else if(LimitHonorDic.ContainsKey(honorName))
                {
                    honorCount = record.ItemCount.Count(p => this.LimitHonorDic[honorName].Any(ps => ps.Name == p.Name));
                }
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

            foreach (var item in LimitItems)
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
}
