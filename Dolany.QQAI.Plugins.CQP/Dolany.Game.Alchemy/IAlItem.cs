using System;

namespace Dolany.Game.Alchemy
{
    public abstract class IAlItem
    {
        public abstract string Name { get; set; }

        public abstract string Description { get;set; }

        public abstract AlCombineNeed CombineNeed { get;set; }

        public abstract int BaseSuccessRate { get; set; }

        public abstract void DoEffect(AlPlayer source, AlPlayer aim, long groupNum);

        public override string ToString()
        {
            var msg = $"{Name}\r";
            msg += $"{Description}\r";
            msg += $"基础成功率：{Math.Round(BaseSuccessRate * 1.0 / 10000, 2)}%";
            msg += $"合成材料清单：{CombineNeed}";

            return msg;
        }
    }
}
