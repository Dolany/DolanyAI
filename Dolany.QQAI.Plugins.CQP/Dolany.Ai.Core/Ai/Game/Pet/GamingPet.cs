using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class GamingPet
    {
        public long QQNum { get; set; }

        public int Level { get; set; }

        public string Name { get; set; }

        public int HP { get; set; }

        public Dictionary<string, int> Skills { get; set; }

        public List<GamingBuff> Buffs { get; set; } = new List<GamingBuff>();
    }

    public class GamingBuff
    {
        public string Name { get; set; }

        public int RemainTurn { get; set; }

        public CheckTrigger Trigger { get; set; }

        public int[] Data { get; set; }
    }

    public enum CheckTrigger
    {
        TurnStart,
        TurnEnd,
        DoSkill,
        BeAttacked
    }
}
