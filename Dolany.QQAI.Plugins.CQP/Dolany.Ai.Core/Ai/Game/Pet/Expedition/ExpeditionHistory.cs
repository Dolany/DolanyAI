using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionHistory : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> SceneDic { get; set; } = new Dictionary<string, int>();

        public int EnduranceConsume { get; set; }

        public int ItemBonusCount { get; set; }

        public int ItemBonusPriceTotal { get; set; }

        public int GoldsTotal { get; set; }

        public int FlavoringTotal { get; set; }

        public static ExpeditionHistory Get(long QQNum)
        {
            var rec = MongoService<ExpeditionHistory>.GetOnly(p => p.QQNum == QQNum);
            if (rec != null)
            {
                return rec;
            }

            rec = new ExpeditionHistory(){QQNum = QQNum};
            MongoService<ExpeditionHistory>.Insert(rec);

            return rec;
        }

        public void Update()
        {
            MongoService<ExpeditionHistory>.Update(this);
        }
    }
}
