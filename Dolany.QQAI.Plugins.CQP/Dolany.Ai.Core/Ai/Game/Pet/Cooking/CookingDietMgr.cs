using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingDietMgr
    {
        public static CookingDietMgr Instance { get; } = new CookingDietMgr();

        private readonly List<CookingDietModel> DietList;

        private CookingDietMgr()
        {
            DietList = CommonUtil.ReadJsonData_NamedList<CookingDietModel>("CookingDietData");
        }
    }

    public class CookingDietModel : INamedJsonModel
    {
        public string Name { get; set; }
    }
}
