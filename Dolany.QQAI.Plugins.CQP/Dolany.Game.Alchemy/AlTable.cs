using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Game.Alchemy
{
    public class AlTable : BaseEntity
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public int FirePattern { get; set; }

        public int WaterPattern { get; set; }

        public int SolidPattern { get; set; }

        public int ThurnderPattern { get; set; }

        public static AlTable GetTable(long QQNum)
        {
            var table = MongoService<AlTable>.Get(t => t.QQNum == QQNum).FirstOrDefault();
            if (table != null)
            {
                return table;
            }

            table = new AlTable()
            {
                QQNum = QQNum
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
        public int AdditionalDirt => ThurnderPattern / 30;

        public AlchemyResultModel DoAlchemy(AlPlayer player, DriftItemRecord itemRecord, IAlItem Item)
        {
            var result = new AlchemyResultModel(){Name = Item.Name};
            var dirtName = MagicDirtHelper.Instance.RandomDirt();
            result.MagicDirtName = dirtName;
            result.MagicDirtCount = AdditionalDirt + 1;

            result.Consume = MaterialReduceRate > CommonUtil.RandInt(10000)
                ? DoMaterialReduce(Item.CombineNeed)
                : Item.CombineNeed;

            // do consume
            // todo

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

        private AlCombineNeed DoMaterialReduce(AlCombineNeed combineNeed)
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
    }

    public class AlchemyResultModel
    {
        public bool IsSuccess { get; set; }

        public AlCombineNeed Consume { private get; set; }

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
