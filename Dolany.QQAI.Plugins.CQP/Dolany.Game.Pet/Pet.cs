using System.Collections.Generic;
using Dolany.Database;

namespace Dolany.Game.Pet
{
    public class Pet : BaseEntity
    {
        public long QQNum { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public int HP { get; set; }

        public List<string> SkillLearned { get; set; } = new List<string>();

        public List<string> FoodPrefer { get; set; } = new List<string>();

        public int Hungry { get; set; }
    }
}
