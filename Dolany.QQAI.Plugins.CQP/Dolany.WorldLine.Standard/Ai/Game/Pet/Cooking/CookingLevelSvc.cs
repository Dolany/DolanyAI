using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking
{
    public class CookingLevelSvc : IDataMgr, IDependency
    {
        private List<CookingLevelModel> CookingLevels;

        public CookingLevelModel this[int level] => CookingLevels.FirstOrDefault(p => p.Level == level);

        public CookingLevelModel LocationLevel(int totalPrice)
        {
            return CookingLevels.LastOrDefault(p => p.NeedPrice <= totalPrice);
        }

        public void RefreshData()
        {
            CookingLevels = CommonUtil.ReadJsonData_NamedList<CookingLevelModel>("Standard/Pet/CookingLevelData").OrderBy(p => p.Level).ToList();
        }
    }

    public class CookingLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int NeedPrice { get; set; }
    }
}
