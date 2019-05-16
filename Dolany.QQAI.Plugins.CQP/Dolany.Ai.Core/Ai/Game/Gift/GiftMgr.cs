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

        public GiftModel this[string GiftName] => GiftDic.ContainsKey(GiftName) ? GiftDic[GiftName] : null;
    }

    public class GiftModel
    {
        public string Name { get; set; }

        public Dictionary<string, int> MaterialDic { get; set; }

        public int GoldNeed { get; set; }

        /// <summary>
        /// 羁绊值
        /// </summary>
        public int Intimate { get; set; }

        /// <summary>
        /// 羁绊值持续天数
        /// </summary>
        public int IntimateDays { get; set; }

        /// <summary>
        /// 魅力值
        /// </summary>
        public int Glamour { get; set; }

        public bool Check(Dictionary<string, int> Mas, int Golds, out string msg)
        {
            msg = string.Empty;
            var result = true;
            msg += $"金币：{Golds}/{GoldNeed}\r";
            if (Golds < GoldNeed)
            {
                result = false;
            }

            foreach (var (key, value) in MaterialDic)
            {
                if (!Mas.ContainsKey(key))
                {
                    msg += $"{key}：0/{value}\r";
                    result = false;
                    continue;
                }

                msg += $"{key}：{Mas[key]}/{value}\r";
                if (Mas[key] < value)
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
