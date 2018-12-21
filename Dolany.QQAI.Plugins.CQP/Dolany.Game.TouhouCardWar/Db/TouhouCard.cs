namespace Dolany.Game.TouhouCardWar.Db
{
    public class TouhouCard
    {
        public string Id { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 卡牌类型:1，英雄；2.符卡；3.天气；4.护符；5.武器
        /// </summary>
        public int Type { get; set; }
        public string Tag { get; set; }
        public string PicPath { get; set; }
        /// <summary>
        /// 元素类型：1.风；2.火；3.水；4.土；5.雷；6.光；7.暗；8.无
        /// </summary>
        int Element { get; set; }
    }
}
