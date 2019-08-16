using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Ai.Game.Advanture.Cave;

namespace Dolany.Ai.Core.Ai.Game.Advanture
{
    public class CaveDataModel : INamedJsonModel
    {
        public string Name { get; set; }
        public int No { get; set; }
        public CaveLevelLimitDataModel LevelLimit { get; set; }
        public Dictionary<string, int> Rates { get; set; }
        public List<CaveMonstersDataModel> Monsters { get; set; }
        public CaveMonstersDataModel Boss { get; set; }
        public List<CaveTrapDataModel> Traps { get; set; }
        public List<CaveTreasureDataModel> Treasures { get; set; }

        public ICave NextCave()
        {
            var type = RandCaveType();
            switch (type)
            {
                case "Monster":
                    var monster = Monsters.RandElement();
                    return new MonsterCave() {Name = monster.Name, Atk = monster.Atk, HP = monster.HP};
                case "Treasure":
                    var treasure = Treasures.RandElement();
                    return new TreasureCave() {Name = treasure.Name, Golds = treasure.Golds, HP = treasure.HP};
                case "Trap":
                    var trap = Traps.RandElement();
                    return new TrapCave() {Name = trap.Name, Atk = trap.Atk};
            }

            return null;
        }

        private string RandCaveType()
        {
            var sum = Rates.Sum(p => p.Value);
            var rand = Rander.RandInt(sum);
            var temp = 0;
            foreach (var (key, value) in Rates)
            {
                if (temp + value >= rand)
                {
                    return key;
                }

                temp += value;
            }

            return Rates.FirstOrDefault().Key;
        }
    }

    public class CaveLevelLimitDataModel
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class CaveMonstersDataModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Atk { get; set; }
        public int HP { get; set; }
        public string Skill { get; set; }
    }

    public class CaveTrapDataModel
    {
        public string Name { get; set; }
        public int Atk { get; set; }
        public string Description { get; set; }
    }

    public class CaveTreasureDataModel
    {
        public string Name { get; set; }
        public int Golds { get; set; }
        public int HP { get; set; }
        public string Description { get; set; }
    }
}
