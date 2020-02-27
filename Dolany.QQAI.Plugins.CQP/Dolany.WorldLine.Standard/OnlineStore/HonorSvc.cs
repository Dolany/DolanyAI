using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.OnlineStore
{
    public class HonorSvc : IDataMgr, IDependency
    {
        public readonly List<HonorModel> HonorList = new List<HonorModel>();

        public List<DriftBottleItemModel> Items = new List<DriftBottleItemModel>();

        private int SumRate;

        public void RefreshData()
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
                if (!value.IsCurLimit)
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

        public IEnumerable<DriftBottleItemModel> CurMonthLimitItems()
        {
            return HonorList.First(h => h is LimitHonorModel limitHonor && limitHonor.IsCurLimit).Items.ToArray();
        }

        public DriftBottleItemModel FindItem(string itemName)
        {
            return Items.FirstOrDefault(i => i.Name == itemName);
        }

        public HonorModel FindHonor(string honorName)
        {
            return HonorList.FirstOrDefault(h => h.Name == honorName);
        }

        public bool IsLimitItem(string itemName)
        {
            var honor = HonorList.FirstOrDefault(h => h.Items.Any(p => p.Name == itemName));
            return honor is LimitHonorModel;
        }

        public bool IsLimitHonor(string honorName)
        {
            var honor = HonorList.First(p => p.Name == honorName);
            return honor is LimitHonorModel;
        }

        public static int GetItemPrice(DriftBottleItemModel item, long qqNum)
        {
            if (item.Rate == 0)
            {
                return 1;
            }

            var price = item.Price;
            if (OSPersonBuff.CheckBuff(qqNum, "疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public int GetHonorPrice(string honorName, long qqNum)
        {
            var honor = FindHonor(honorName);
            var price = honor.Items.Sum(item => item.Rate == 0 ? 1 : item.Price) * 3 / 2;

            if (OSPersonBuff.CheckBuff(qqNum, "疏雨"))
            {
                price += price * 20 / 100;
            }
            return price;
        }

        public string FindHonorFullName(string itemName)
        {
            return HonorList.FirstOrDefault(p => p.Items.Any(i => i.Name == itemName))?.FullName;
        }

        public string FindHonorName(string itemName)
        {
            return HonorList.FirstOrDefault(p => p.Items.Any(i => i.Name == itemName))?.Name;
        }

        private DriftBottleItemModel LocalateItem(int index)
        {
            var totalSum = 0;
            foreach (var item in Items)
            {
                if (index < totalSum + item.Rate)
                {
                    return item;
                }

                totalSum += item.Rate;
            }

            return Items.First();
        }

        public DriftBottleItemModel RandItem()
        {
            return LocalateItem(Rander.RandInt(SumRate));
        }

        public List<DriftBottleItemModel> RandItems(int count)
        {
            var list = new List<DriftBottleItemModel>();
            for (var i = 0; i < count; i++)
            {
                list.Add(RandItem());
                Thread.Sleep(50);
            }

            return list;
        }

        public double ItemRate(DriftBottleItemModel item)
        {
            if (item.Rate == 0)
            {
                return 0;
            }
            return Math.Round(item.Rate * 1.0 / SumRate * 100, 2);
        }

        public IList<string> GetOrderedItemsStr(Dictionary<string, int> items)
        {
            var itemHonorDic = items.Select(i => new {Honor = FindHonor(FindHonorName(i.Key)), i.Key, i.Value})
                .GroupBy(p => p.Honor)
                .ToDictionary(p => p.Key, p => p.ToList());
            var list = itemHonorDic.Select(kv =>
                $"{kv.Key.FullName}({kv.Value.Count}/{kv.Key.Items.Count}):{string.Join(",", kv.Value.Select(p => $"{p.Key}*{p.Value}"))}");
            return list.ToList();
        }
    }
}
