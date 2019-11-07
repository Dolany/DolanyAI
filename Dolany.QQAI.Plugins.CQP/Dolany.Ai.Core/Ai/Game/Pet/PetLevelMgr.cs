using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetLevelMgr
    {
        public static PetLevelMgr Instance { get; } = new PetLevelMgr();

        private readonly Dictionary<int, PetLevelModel> LevelDic;

        public PetLevelModel this[int level] => LevelDic[level];

        private PetLevelMgr()
        {
            LevelDic = CommonUtil.ReadJsonData_NamedList<PetLevelModel>("PetLevelData")
                .ToDictionary(p => p.Level, p => p);
        }

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
