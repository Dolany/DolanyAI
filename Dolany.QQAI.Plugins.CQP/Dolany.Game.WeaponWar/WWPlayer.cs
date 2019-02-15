using System;
using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Game.WeaponWar
{
    public class WWPlayer : BaseEntity
    {
        public long QQNum { get; set; }

        public int MaxHP { get; set; }

        public int HP { get; set; }

        public int WeaponCapacity { get; set; }

        public int ShielderCapacity { get; set; }

        public IList<WeaponModel> Weapons { get; set; }

        public IList<ShielderModel> Shielders { get; set; }

        public Dictionary<string, int> SpecialMaterialDic { get; set; }

        public IList<DragModel> Drags { get; set; }
    }

    public class WeaponModel
    {
        public string Name { get; set; }

        public DateTime CoolTime { get; set; }

        public int Attack { get; set; }

        public int MaxEndurance { get; set; }

        public int Endurance { get; set; }

        public int Volume { get; set; }
    }

    public class ShielderModel
    {
        public string Name { get; set; }

        public DateTime CoolTime { get; set; }

        public int Defence { get; set; }

        public int MaxEndurance { get; set; }

        public int Endurance { get; set; }

        public int Volume { get; set; }
    }

    public class DragModel
    {
        public string Name { get; set; }

        public DateTime ExpiryTime { get; set; }
    }
}
