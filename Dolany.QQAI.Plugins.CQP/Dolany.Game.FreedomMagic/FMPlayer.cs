using System.Collections.Generic;

namespace Dolany.Game.FreedomMagic
{
    public class FMPlayer
    {
        public long QQNum { get; set; }

        public List<FMMagic> Magics { get; set; }

        public int MagicVolume { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }

        public int MaxHP { get; set; }

        public int MaxMP { get; set; }

        public int WaitTime { get; set; }
    }
}
