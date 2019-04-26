using System.Collections.Generic;

namespace Dolany.Ai.Core.Ai.Game.Cooking
{
    public class CookingBookModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public CookingDietModel[] Diets { get; set; }

        public string[] ExchangeHonors { get; set; }
    }

    public class CookingDietModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<string, int> Materials { get; set; }

        public int Sugar { get; set; }

        public int SAN { get; set; }

        public int Health { get; set; }
    }
}
