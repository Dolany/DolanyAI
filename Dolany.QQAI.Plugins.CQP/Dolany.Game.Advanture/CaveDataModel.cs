﻿using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Game.Advanture.Cave;

namespace Dolany.Game.Advanture
{
    public class CaveDataModel
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
                    var monster = Monsters[CommonUtil.RandInt(Monsters.Count)];
                    return new MonsterCave() {Name = monster.Name, Atk = monster.Atk, HP = monster.HP};
                case "Treasure":
                    var treasure = Treasures[CommonUtil.RandInt(Treasures.Count)];
                    return new TreasureCave() {Name = treasure.Name, Golds = treasure.Golds, HP = treasure.HP};
                case "Trap":
                    var trap = Traps[CommonUtil.RandInt(Traps.Count)];
                    return new TrapCave() {Name = trap.Name, Atk = trap.Atk};
            }

            return null;
        }

        private string RandCaveType()
        {
            var sum = Rates.Sum(p => p.Value);
            var rand = CommonUtil.RandInt(sum);
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