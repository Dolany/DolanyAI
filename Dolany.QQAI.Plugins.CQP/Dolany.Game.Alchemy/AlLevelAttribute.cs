using System;

namespace Dolany.Game.Alchemy
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AlLevelAttribute : Attribute
    {
        public int Level { get; set; }

        public string Description { get; set; }

        public int BaseSuccessRate { get; set; }
    }
}
