using System;

namespace Dolany.Game.Chess
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChessEffectAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
