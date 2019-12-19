using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingLevelMgr
    {
        public static CookingLevelMgr Instance { get; } = new CookingLevelMgr();

        private readonly List<CookingLevelModel> CookingLevels;

        public CookingLevelModel this[int level] => CookingLevels.FirstOrDefault(p => p.Level == level);

        private CookingLevelMgr()
        {
            CookingLevels = CommonUtil.ReadJsonData_NamedList<CookingLevelModel>("Pet/CookingLevelData").OrderBy(p => p.Level).ToList();
        }

        public CookingLevelModel LocationLevel(int totalPrice)
        {
            return CookingLevels.LastOrDefault(p => p.NeedPrice <= totalPrice);
        }
    }

    public class CookingLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int NeedPrice { get; set; }
    }
}
