namespace Dolany.Game.TouhouCardWar.Db
{
    public class HeroLines
    {
        public string Id { get; set; }
        public int HeroNo { get; set; }
        /// <summary>
        /// 场景：1.入场；2.死亡
        /// </summary>
        public int Scene { get; set; }
        /// <summary>
        /// 标签：0为通用台词，其他为对阵英雄编号
        /// </summary>
        public int Tag { get; set; }
    }
}
