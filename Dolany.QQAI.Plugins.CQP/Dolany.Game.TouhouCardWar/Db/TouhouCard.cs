namespace Dolany.Game.TouhouCardWar.Db
{
    public class TouhouCard
    {
        public string Id { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 卡牌类型:1，英雄；2.符卡；3.天气；4.护符；5.武器；6.反击
        /// </summary>
        public int Type { get; set; }
        public string Tag { get; set; }
        public string PicPath { get; set; }

        /// <summary>
        /// 颜色类型：1.青；2.白；3.朱；4.玄；5.黄
        /// </summary>
        public int Color { get; set; }
    }
}
