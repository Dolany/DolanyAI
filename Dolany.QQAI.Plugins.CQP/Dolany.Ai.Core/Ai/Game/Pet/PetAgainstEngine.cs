using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetAgainstEngine
    {
        public long GroupNum { get; set; }

        public string BindAi { get; set; }

        public GamingPet SelfPet { get; set; }

        public GamingPet AimPet { get; set; }

        public void StartGame()
        {

        }
    }

    public class GamingPet
    {
        public long QQNum { get; set; }

        public string Name { get; set; }

        public int HP { get; set; }

        public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();

        public List<GamingBuff> Buffs { get; set; } = new List<GamingBuff>();
    }

    public class GamingBuff
    {
        public string Name { get; set; }

        public int RemainTurn { get; set; }

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
