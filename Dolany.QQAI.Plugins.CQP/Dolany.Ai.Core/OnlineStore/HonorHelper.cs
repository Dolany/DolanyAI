using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.OnlineStore
{
    public class HonorHelper
    {
        public static HonorHelper Instance { get; set; } = new HonorHelper();

        public readonly List<HonorModel> HonorList = new List<HonorModel>();

        private List<DriftBottleItemModel> Items = new List<DriftBottleItemModel>();

        private int SumRate;

        private HonorHelper()
        {
            Refresh();
        }

        public void Refresh()
        {
            var HonorDic = CommonUtil.ReadJsonData<Dictionary<string, DriftBottleItemModel[]>>("driftBottleItemData");
            var LimitHonors = CommonUtil.ReadJsonData<Dictionary<string, LimitHonorModel>>("driftBottleItemData_Limit");

            HonorList.Clear();
            HonorList.AddRange(HonorDic.Select(hd => new HonorModel()
            {
                Name = hd.Key,
                Items = hd.Value.ToList()
            }));

            foreach (var (key, value) in LimitHonors)
            {
                if (value.Year != DateTime.Now.Year || value.Month != DateTime.Now.Month)
                {
                    foreach (var item in value.Items)
                    {
                        item.Rate = 0;
                    }
                }

                value.Name = key;
                HonorList.Add(value);
            }

            Items = HonorList.SelectMany(p => p.Items).ToList();
            SumRate = HonorList.Sum(h => h.Items.Sum(hi => hi.Rate));
        }

        public DriftBottleItemModel[] CurMonthLimitItems()
        {
            return HonorList.First(h => h is LimitHonorModel limitHonor && limitHonor.Year == DateTime.Now.Year && limitHonor.Month == DateTime.Now.Month).Items
                .ToArray();
        }

        public DriftBottleItemModel FindItem(string name)
        {
            return Items.FirstOrDefault(i => i.Name == name);
        }

        public HonorModel FindHonor(string name)
        {
            return HonorList.FirstOrDefault(h => h.Name == name);
        }

        public bool IsLimit(string itemName)
        {
            var honor = HonorList.First(h => h.Items.Any(p => p.Name == itemName));
            return honor is LimitHonorModel;
        }

        public int GetItemPrice(DriftBottleItemModel item, long qqNum)
        {
            if (item.Rate == 0)
            {
                return 1;
            }

            var price = item.Price;
            var osPerson = OSPerson.GetPerson(qqNum);
            if (osPerson.CheckBuff("疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public int GetHonorPrice(string honorName, long qqNum)
        {
            var honor = FindHonor(honorName);
            var price = honor.Items.Sum(item =>
            {
                if (item.Rate == 0)
                {
                    return 1;
                }

                return item.Price;
            }) * 3 / 2;

            var osPerson = OSPerson.GetPerson(qqNum);
            if (osPerson.CheckBuff("疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public string FindHonorName(string itemName)
        {
            return HonorList.FirstOrDefault(p => p.Items.Any(i => i.Name == itemName))?.FullName;
        }

        public (string msg, bool IsNewHonor) CheckHonor(DriftItemRecord record, string itemName)
        {
            if (record?.ItemCount == null)
            {
                return (string.Empty, false);
            }

            var honorCount = 0;
            var honor = HonorList.First(h => h.Items.Any(i => i.Name == itemName));
            if (record.HonorList == null || !record.HonorList.Contains(honor.Name))
            {
                honorCount = record.ItemCount.Count(p => honor.Items.Any(ps => ps.Name == p.Name));
            }

            if (honorCount == 0)
            {
                return (string.Empty, false);
            }

            if (honorCount < honor.Items.Count)
            {
                return ($"成就 {honor.FullName} 完成度：{honorCount}/{honor.Items.Count}", false);
            }

            return ($"恭喜你解锁了成就 {honor.FullName}! (集齐物品：{string.Join("，", honor.Items.Select(p => p.Name))})", true);
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
            var itemHonorDic = items.Select(i => new {Honor = FindHonorName(i.Name), i.Name, i.Count})
                .GroupBy(p => p.Honor)
                .ToDictionary(p => p.Key, p => p.ToList());
            var list = itemHonorDic.Select(kv => $"{kv.Key}:{string.Join(",", kv.Value.Select(p => $"{p.Name}({p.Count})"))}");
            return list.ToList();
        }
    }
}
