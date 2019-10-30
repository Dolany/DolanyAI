using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> CookedDietDic { get; set; }

        public Dictionary<string, int> FlavoringDic { get; set; }

        public static CookingRecord Get(long QQNum)
        {
            var record = MongoService<CookingRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new CookingRecord(){QQNum = QQNum};
            MongoService<CookingRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            CookedDietDic.Remove(p => p == 0);
            FlavoringDic.Remove(p => p == 0);

            MongoService<CookingRecord>.Update(this);
        }
    }
}
