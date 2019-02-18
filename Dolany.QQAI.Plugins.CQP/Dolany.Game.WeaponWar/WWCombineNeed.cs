using System.Collections.Generic;

namespace Dolany.Game.WeaponWar
{
    public class WWCombineNeed
    {
        public IList<string> WeaponNeed { get; set; }

        public IList<string> ShieldNeed { get; set; }

        public Dictionary<string, int> ItemNeed { get; set; }

        public Dictionary<string, int> SpecialMaterialNeed { get; set; }

        public int GoldNeed { get; set; }
    }
}
