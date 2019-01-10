namespace Dolany.Database.Incantation
{
    using System;

    public class IncaCharactor
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long QQNum { get; set; }

        public string CharactorName { get; set; }

        public int Level { get; set; }

        public int MaxIncaCount { get; set; }

        public int MaxHP { get; set; }

        /// <summary>
        /// 属性-力量，影响最大生命值
        /// </summary>
        public int Property_Strength { get; set; }

        /// <summary>
        /// 属性-敏捷，影响冷却时间和吟唱时间
        /// </summary>
        public int Property_Agility { get; set; }

        /// <summary>
        /// 属性-智力，影响法术等级
        /// </summary>
        public int Property_Intellect { get; set; }
    }
}
