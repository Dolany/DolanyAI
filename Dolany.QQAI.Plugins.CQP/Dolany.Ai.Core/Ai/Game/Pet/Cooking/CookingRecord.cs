using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<string> LearndDietMenu { get; set; } = new List<string>();

        public Dictionary<string, int> CookedDietDic { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> FlavoringDic { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, int> CookingHistory { get; set; } = new Dictionary<string, int>();

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

        public void AddDiet(string name, int count = 1)
        {
            if (!CookedDietDic.ContainsKey(name))
            {
                CookedDietDic.Add(name, 0);
            }

            CookedDietDic[name] += count;

            if (CookingHistory == null)
            {
                CookingHistory = new Dictionary<string, int>();
            }

            if (!CookingHistory.ContainsKey(name))
            {
                CookingHistory.Add(name, 0);
            }

            CookingHistory[name] += count;
        }

        public void DietConsume(string name, int count = 1)
        {
            if (CookedDietDic.ContainsKey(name))
            {
                CookedDietDic[name] -= count;
            }
        }

        public bool CheckDiet(string name, int count = 1)
        {
            return CookedDietDic.ContainsKey(name) && CookedDietDic[name] >= count;
        }

        public bool CheckFlavorings(Dictionary<string, int> fDic)
        {
            if (FlavoringDic.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var (name, count) in fDic)
            {
                if (!FlavoringDic.ContainsKey(name) || FlavoringDic[name] < count)
                {
                    return false;
                }
            }

            return true;
        }

        public void FlavoringConsume(Dictionary<string, int> fDic)
        {
            if (fDic.IsNullOrEmpty())
            {
                return;
            }

            foreach (var (name, count) in fDic)
            {
                FlavoringDic[name] -= count;
            }
        }

        public void FlavoringIncome(string flavoringName, int count = 1)
        {
            if (!FlavoringDic.ContainsKey(flavoringName))
            {
                FlavoringDic.Add(flavoringName, 0);
            }

            FlavoringDic[flavoringName] += count;
        }

        public void Update()
        {
            CookedDietDic.Remove(p => p == 0);
            FlavoringDic.Remove(p => p == 0);

            MongoService<CookingRecord>.Update(this);
        }
    }
}
