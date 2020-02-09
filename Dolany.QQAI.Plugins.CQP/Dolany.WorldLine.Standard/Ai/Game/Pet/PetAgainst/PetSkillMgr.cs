﻿using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst
{
    public class PetSkillMgr : IDataMgr
    {
        public static PetSkillMgr Instance { get; } = new PetSkillMgr();

        public List<PetSkillModel> AllSkills;

        public PetSkillModel this[string name]
        {
            get { return AllSkills.FirstOrDefault(p => p.Name == name); }
        }

        private PetSkillMgr()
        {
            RefreshData();
            DataRefresher.Instance.Register(this);
        }

        public void RefreshData()
        {
            AllSkills = CommonUtil.ReadJsonData_NamedList<PetSkillModel>("Pet/PetSkillData");
        }
    }

    public class PetSkillModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<string, int[]> Data { get; set; }

        public int LearnLevel { get; set; }

        public string CommDesc
        {
            get
            {
                var desc = new string(Description);
                foreach (var (key, values) in Data)
                {
                    desc = desc.Replace(key, string.Join("/", values));
                }

                return desc;
            }
        }
    }
}
