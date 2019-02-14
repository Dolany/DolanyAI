using System.Collections.Generic;

namespace Dolany.Game.WeaponWar
{
    public class WWCombineNeed
    {
        public Dictionary<string, int> WWWeaponNeed { get; set; }

        public Dictionary<string, int> WWShielderNeed { get; set; }

        public Dictionary<string, int> ItemNeed { get; set; }

        public Dictionary<string, int> SpecialMaterialNeed { get; set; }

        public int GoldNeed { get; set; }
    }
}
