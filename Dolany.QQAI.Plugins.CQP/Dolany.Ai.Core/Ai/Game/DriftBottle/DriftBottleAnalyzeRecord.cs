using System;
using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.DriftBottle
{
    public class DriftBottleAnalyzeRecord : DbBaseEntity
    {
        public string DateStr { get; set; }

        public Dictionary<string, int> ItemDic { get; set; } = new Dictionary<string, int>();

        public static DriftBottleAnalyzeRecord GetToday()
        {
            var dstr = DateTime.Now.ToString("yyyyMMdd");
            var rec = MongoService<DriftBottleAnalyzeRecord>.GetOnly(p => p.DateStr == dstr);
            if (rec != null)
            {
                return rec;
            }

            rec = new DriftBottleAnalyzeRecord(){DateStr = dstr};
            MongoService<DriftBottleAnalyzeRecord>.Insert(rec);

            return rec;
        }

        public static void Record(string item, int count = 1)
        {
            var dstr = DateTime.Now.ToString("yyyyMMdd");
            var rec = GetToday();

            if (!rec.ItemDic.ContainsKey(item))
            {
                rec.ItemDic.Add(item, 0);
            }

            rec.ItemDic[item] += count;
            rec.Update();
        }

        public void Update()
        {
            MongoService<DriftBottleAnalyzeRecord>.Update(this);
        }
    }
}
