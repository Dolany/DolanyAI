namespace Dolany.Database.Incantation
{
    using System;

    public class IncaMagic
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long QQNum { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 咒文
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 成功率，1~100
        /// </summary>
        public int SuccessRate { get; set; }

        /// <summary>
        /// 冷却时间（要求 >= 1）
        /// </summary>
        public int CD { get; set; }
    }
}
