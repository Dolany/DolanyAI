using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SEWeaponMgr
    {
        public static SEWeaponMgr Instance { get; } = new SEWeaponMgr();

        private List<SEWeaponModel> Weapons;
        private readonly Dictionary<int, int> LevelUpExpDic;

        private SEWeaponMgr()
        {
            Weapons = CommonUtil.ReadJsonData_NamedList<SEWeaponModel>("SE/SEWeaponData");
            LevelUpExpDic = CommonUtil.ReadJsonData<Dictionary<string, int>>("SE/SELevelUpExpData").ToDictionary(p => int.Parse(p.Key), p => p.Value);
        }

        public int TopLevel => LevelUpExpDic.Keys.Max();
    }

    public class SEWeaponModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<SEWeaponLevelDataModel> LevelDatas { get; set; }
    }

    public class SEWeaponLevelDataModel
    {
        public int Level { get; set; }

        public int Attack { get; set; }

        public string[] NewSkills { get; set; }
    }
}
