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

        public string LastSkill { get; set; }
    }

    public class GamingEffect
    {
        public string Name { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }

    public class GamingBuff : GamingEffect
    {
        public int RemainTurn { get; set; }

        public CheckTrigger Trigger { get; set; }
    }

    public enum CheckTrigger
    {
        TurnStart,
        TurnEnd,
        DoSkill,
        BeAttacked,
        PhyAttackFix,
        MagicAttackFix,
        PoisionAttackFix,
        PhyDefenceFix,
        MagicDefenceFix,
        PoisionDefenceFix
    }
}
