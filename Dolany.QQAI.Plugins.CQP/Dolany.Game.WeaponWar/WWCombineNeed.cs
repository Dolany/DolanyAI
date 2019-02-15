using System.Collections.Generic;

namespace Dolany.Game.WeaponWar
{
    public class WWCombineNeed
    {
        public IList<string> WWWeaponNeed { get; set; }

        public IList<string> WWShielderNeed { get; set; }

        public Dictionary<string, int> ItemNeed { get; set; }

        public Dictionary<string, int> SpecialMaterialNeed { get; set; }

        public int GoldNeed { get; set; }
    }
}
