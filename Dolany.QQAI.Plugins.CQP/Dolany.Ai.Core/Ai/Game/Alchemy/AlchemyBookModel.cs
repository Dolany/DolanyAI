using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.Game.Alchemy
{
    public class AlchemyBookModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public AlchemyDietModel[] Diets { get; set; }

        public string[] ExchangeHonors { get; set; }
    }

    public class AlchemyDietModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<string, int> Materials { get; set; }

        public Dictionary<string, int> Flavorings { get; set; }

        public Dictionary<AlchemyToolTypeEnum, int> ToolLevelRequire { get; set; }

        public int Sugar { get; set; }

        public int SAN { get; set; }

        public int Health { get; set; }
    }
}
