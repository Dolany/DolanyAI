using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet
{
    public class PetLevelSvc : IDataMgr, IDependency
    {
        private Dictionary<int, PetLevelModel> LevelDic;

        public PetLevelModel this[int level] => LevelDic[level];

        public int ExpToGolds(int level, int exp)
        {
            var petAssert = 0;
            for (var i = 1; i < level; i++)
            {
                petAssert += this[i].Exp * 10;
            }

            petAssert += exp * 10;
            return petAssert;
        }

        public void RefreshData()
        {
            LevelDic = CommonUtil.ReadJsonData_NamedList<PetLevelModel>("Pet/PetLevelData")
                .ToDictionary(p => p.Level, p => p);
        }
    }

    public class PetLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Exp { get; set; }

        public int HP { get; set; }

        public int Endurance { get; set; }

        public int Level => int.Parse(Name);
    }
}
