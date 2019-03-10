using System;
using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Game.Alchemy.MagicBook.初级炼金手册
{
    public class 碎火水晶 : IAlItem
    {
        public override string Name { get; set; } = "碎火水晶";
        public override string Description { get; set; } = "来着碎火地狱的水晶，可以提升所有魔纹1点";
        public override AlCombineNeed CombineNeed { get; set; } = new AlCombineNeed()
        {
            NormalItemNeed = new Dictionary<string, int>(){{"方形的石头", 1}},
            GoldsNeed = 50
        };

        public override int BaseSuccessRate { get; set; } = 6000;
        public override void DoEffect(AlPlayer source, AlPlayer aim, long groupNum)
        {
            var table = AlTable.GetTable(aim.QQNum);
            var tableLevel = AlTableHelper.Instance[table.Level];

            var fire = Math.Min(tableLevel.MaxFire, table.FirePattern + 1);
            var water = Math.Min(tableLevel.MaxWater, table.WaterPattern + 1);
            var solid = Math.Min(tableLevel.MaxSolid, table.SolidPattern + 1);
            var thunder = Math.Min(tableLevel.MaxThunder, table.ThunderPattern + 1);

            CommonUtil.MsgSendBack(groupNum, source.QQNum, 
                $"提升成功！火魔纹：{fire},水魔纹：{water},土魔纹：{solid},雷魔纹：{thunder}", false);
            source.ItemConsume("碎火水晶", 1);

            source.Update();
            table.Update();
        }
    }
}
