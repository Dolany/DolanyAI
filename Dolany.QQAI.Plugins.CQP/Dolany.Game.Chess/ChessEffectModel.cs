using System;

namespace Dolany.Game.Chess
{
    public class ChessEffectModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Action Method { get; set; }

        public bool IsChecked { get; set; } = false;
    }
}
