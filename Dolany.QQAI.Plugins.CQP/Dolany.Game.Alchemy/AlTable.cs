using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Game.OnlineStore;

namespace Dolany.Game.Alchemy
{
    public class AlTable : BaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public int FirePattern { get; set; }

        public int WaterPattern { get; set; }

        public int SolidPattern { get; set; }

        public int ThunderPattern { get; set; }

        public static AlTable GetTable(long QQNum)
        {
            var table = MongoService<AlTable>.Get(t => t.QQNum == QQNum).FirstOrDefault();
            if (table != null)
            {
                return table;
            }

            table = new AlTable()
            {
                QQNum = QQNum,
                Level = 1
            };
            MongoService<AlTable>.Insert(table);

            return table;
        }

        // 暴击率
        public int DoubleRate => (int) (1.0 * FirePattern / (FirePattern + 200) * 10000);

        // 材料减免率
        public int MaterialReduceRate => (int) (1.0 * WaterPattern / (WaterPattern + 200) * 10000);

        // 成功率提升
        public int SuccessUpRate => (int) (1.0 * SolidPattern / (SolidPattern + 200) * 10000);

        // 额外的魔尘
        public int AdditionalDirt => ThunderPattern / 30;

        public bool IsMaxLevel => AlTableHelper.Instance.MaxLevel == Level;

        public bool CanUpgrade
        {
            get
            {
                var tableLevelData = AlTableHelper.Instance[Level];
                var fullCount = 0;
                if (FirePattern == tableLevelData.MaxFire)
                {
                    fullCount++;
                }

                if (WaterPattern == tableLevelData.MaxWater)
                {
                    fullCount++;
                }

                if (SolidPattern == tableLevelData.MaxSolid)
                {
                    fullCount++;
                }

                if (ThunderPattern == tableLevelData.MaxThunder)
                {
                    fullCount++;
                }

                return fullCount >= 3;
            }
        }

        public AlchemyResultModel DoAlchemy(AlPlayer player, DriftItemRecord itemRecord, IAlItem Item, OSPerson osPerson)
        {
            var result = new AlchemyResultModel(){Name = Item.Name};
            var dirtName = MagicDirtHelper.Instance.RandomDirt();
            result.MagicDirtName = dirtName;
            result.MagicDirtCount = AdditionalDirt + 1;

            result.Consume = MaterialReduceRate > CommonUtil.RandInt(10000)
                ? DoMaterialNeedReduce(Item.CombineNeed)
                : Item.CombineNeed;

            result.Consume.DoConsume(player, osPerson, itemRecord);

            if (Item.BaseSuccessRate + SuccessUpRate > CommonUtil.RandInt(10000))
            {
                result.IsSuccess = true;
            }

            if (result.IsSuccess && DoubleRate > CommonUtil.RandInt(10000))
            {
                result.Count = 2;
            }

            return result;
        }

        // 计算所需材料减少
        private AlCombineNeed DoMaterialNeedReduce(AlCombineNeed combineNeed)
        {
            var result = combineNeed;
            var sumRate = result.AlItemNeed.Count + result.MagicDirtNeed.Count + result.NormalItemNeed.Count;
            var index = CommonUtil.RandInt(sumRate);
            if (result.AlItemNeed.Count > index)
            {
                var (key, _) = result.AlItemNeed.ElementAt(index);
                result.AlItemNeed.Remove(key);
                return result;
            }

            if (result.AlItemNeed.Count + result.MagicDirtNeed.Count > index)
            {
                var (key, _) = result.MagicDirtNeed.ElementAt(index - result.AlItemNeed.Count);
                result.MagicDirtNeed.Remove(key);
                return result;
            }

            var (k, _) = result.NormalItemNeed.ElementAt(index - result.AlItemNeed.Count - result.MagicDirtNeed.Count);
            result.NormalItemNeed.Remove(k);
            return result;
        }

        public override string ToString()
        {
            var tableLevelModel = AlTableHelper.Instance[Level];

            var msg = $"Level:{Level}\r";
            msg += $"火魔纹：{FirePattern}/{tableLevelModel.MaxFire}(暴击率{Math.Round(DoubleRate * 1.0 / 100, 2)}%)\r";
            msg += $"水魔纹：{WaterPattern}/{tableLevelModel.MaxWater}(材料减免率{Math.Round(MaterialReduceRate * 1.0 / 100, 2)}%)\r";
            msg += $"土魔纹：{SolidPattern}/{tableLevelModel.MaxSolid}(成功率提升{Math.Round(SuccessUpRate * 1.0 / 100, 2)}%)\r";
            msg += $"雷魔纹：{ThunderPattern}/{tableLevelModel.MaxThunder}(额外的魔尘{AdditionalDirt})";

            return msg;
        }

        public void Update()
        {
            MongoService<AlTable>.Update(this);
        }
    }

    public class AlchemyResultModel
    {
        public bool IsSuccess { get; set; }

        public AlCombineNeed Consume {   get; set; }

        public int Count { private get; set; } = 1;

        public string Name { private get; set; }

        public string MagicDirtName { private get; set; }

        public int MagicDirtCount { private get; set; }

        public override string ToString()
        {
            var msg = IsSuccess ?
                $"炼成成功！你获得了{Name}*{Count},{MagicDirtName}*{MagicDirtCount}！"
                : $"炼成失败！你获得了{MagicDirtName}*{MagicDirtCount}！";
            msg += "\r你消耗了材料：";
            var list = new List<string>();
            foreach (var alItem in Consume.AlItemNeed)
            {
                var (name, count) = alItem;
                list.Add($"{name}*{count}");
            }

            foreach (var magicDirt in Consume.MagicDirtNeed)
            {
                var (name, count) = magicDirt;
                list.Add($"{name}*{count}");
            }

            foreach (var nitem in Consume.NormalItemNeed)
            {
                var (name, count) = nitem;
                list.Add($"{name}*{count}");
            }

            msg += string.Join(",", list);
            return msg;
        }
    }
}
