using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Gift
{
    public class GiftMgr
    {
        public static GiftMgr Instance { get; } = new GiftMgr();

        public Dictionary<string, GiftModel> GiftDic { get; }

        private GiftMgr()
        {
            GiftDic = CommonUtil.ReadJsonData<Dictionary<string, GiftModel>>("GiftData");
            foreach (var (key, value) in GiftDic)
            {
                value.Name = key;
            }
        }
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
