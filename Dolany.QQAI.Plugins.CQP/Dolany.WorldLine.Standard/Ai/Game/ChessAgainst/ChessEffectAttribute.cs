using System;

namespace Dolany.WorldLine.Standard.Ai.Game.ChessAgainst
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChessEffectAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
