using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;

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
                .ToDictionary(p => int.Parse(p.Name), p => p);
        }
    }

    public static class PetExtent
    {
        public static void ExtGain(this PetRecord pet, MsgInformationEx MsgDTO, int exp)
        {

        }
    }

    public class PetLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Exp { get; set; }

        public int Level => int.Parse(Name);
    }
}
