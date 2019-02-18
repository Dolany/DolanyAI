using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Game.WeaponWar
{
    public class WWWeaponHelper
    {


        public static WWWeaponHelper Instance { get; set; } = new WWWeaponHelper();

        private WWWeaponHelper()
        {

        }
    }

    public class WeaponModel
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public IList<WeaponLevelModel> LvlList { get; set; }

        public int Weight { get; set; }

        public string BulletKind { get; set; }

        public WWCombineNeed CombineNeed { get; set; }
    }

    public class WeaponLevelModel
    {
        public int Lv { get; set; }

        public int HP { get; set; }

        public int Attack { get; set; }

        public int CD { get; set; }

        public WWCombineNeed LvUpNeed { get; set; }
    }
}
