using System;
using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Game.Alchemy.MagicBook.初级炼金手册
{
    public class 梦魇水晶 : IAlItem
    {
        public override string Name { get; set; } = "梦魇水晶";
        public override string Description { get; set; } = "梦境之力凝结的水晶，能用于提升3点水魔纹";
        public override AlCombineNeed CombineNeed { get; set; } = new AlCombineNeed()
        {
            NormalItemNeed = new Dictionary<string, int>(){{"很厚的书", 1}, {"一撮兽毛", 1}},
            MagicDirtNeed = new Dictionary<string, int>(){{"黑魔粉", 1}}
        };

        public override int BaseSuccessRate { get; set; } = 5000;

        public override void DoEffect(AlPlayer source, AlPlayer aim, long groupNum)
        {
            var table = AlTable.GetTable(aim.QQNum);
            var tableLevel = AlTableHelper.Instance[table.Level];
            if (tableLevel.MaxWater <= table.WaterPattern)
            {
                CommonUtil.MsgSendBack(groupNum, source.QQNum, "无法提升，水魔纹已达到当前等级的最大值！", true);
                return;
            }

            var value = Math.Min(tableLevel.Level, table.WaterPattern + 3);
            CommonUtil.MsgSendBack(groupNum, source.QQNum, $"提升成功！水魔纹提升到了 {value} 点！", false);
            table.WaterPattern = value;

            source.ItemConsume("梦魇水晶", 1);

            source.Update();
            table.Update();
        }
    }
}
