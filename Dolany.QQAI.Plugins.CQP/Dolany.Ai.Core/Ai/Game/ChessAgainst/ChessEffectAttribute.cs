using System;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChessEffectAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
