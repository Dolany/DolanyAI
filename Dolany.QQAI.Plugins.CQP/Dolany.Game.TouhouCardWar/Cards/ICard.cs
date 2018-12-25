namespace Dolany.Game.TouhouCardWar.Cards
{
    public interface ICard
    {
        int No { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        /// <summary>
        /// 卡牌类型:1，英雄；2.符卡；3.天气；4.护符；5.武器；6.反击；7.弹幕
        /// </summary>
        int Type { get; set; }
        string Tag { get; set; }
        string PicPath { get; set; }

        /// <summary>
        /// 颜色类型：1.青；2.白；3.朱；4.玄；5.黄
        /// </summary>
        int Color { get; set; }
    }
}
