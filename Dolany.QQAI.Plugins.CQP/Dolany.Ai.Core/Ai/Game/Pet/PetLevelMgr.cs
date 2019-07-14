﻿using System.Collections.Generic;
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
                .ToDictionary(p => int.Parse(p.Name), p => p);
        }
    }

    public class PetLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Exp { get; set; }

        public int HP { get; set; }

        public int Level => int.Parse(Name);
    }
}
