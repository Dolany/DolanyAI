using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    public class GiftMgr
    {

    }

    public class GiftModel
    {
        public string Name { get; set; }

        public Dictionary<string, int> MaterialDic { get; set; }

        /// <summary>
        /// 好感度
        /// </summary>
        public int Intimate { get; set; }

        /// <summary>
        /// 好感度持续天数
        /// </summary>
        public int IntimateDays { get; set; }

        public int Glamour { get; set; }
    }
}
