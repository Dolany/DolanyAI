namespace Dolany.Database.Touhou
{
    public class TouhouCard
    {
        public string Id { get; set; }
        int No { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        /// <summary>
        /// 卡牌类型:1.英雄；2.随从；3.符卡；4.武器
        /// </summary>
        int Type { get; set; }
        string Tag { get; set; }
        string PicPath { get; set; }

        /// <summary>
        /// 颜色类型：1.天；2.地；3.人；4.中立
        /// </summary>
        int Color { get; set; }
    }
}
