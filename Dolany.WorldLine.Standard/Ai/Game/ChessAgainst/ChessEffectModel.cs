using System;

namespace Dolany.WorldLine.Standard.Ai.Game.ChessAgainst
{
    public class ChessEffectModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Action Method { get; set; }

        public bool IsChecked { get; set; } = false;
    }
}
