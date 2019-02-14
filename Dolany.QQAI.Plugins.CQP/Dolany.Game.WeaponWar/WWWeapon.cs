using Dolany.Database;

namespace Dolany.Game.WeaponWar
{
    public class WWWeapon : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int BasicAttack { get; set; }

        public int BasicEndurance { get; set; }

        public WWCombineNeed Combine { get; set; }

        public int LevelNeed { get; set; }

        public int CoolMinutes { get; set; }

        public int Volume { get; set; }
    }
}
