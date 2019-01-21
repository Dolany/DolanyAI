using System.Collections.Generic;
using System.Linq;
using Dolany.Game.MagicCleanUp.Magic;

namespace Dolany.Game.MagicCleanUp
{
    public class Player
    {
        public int TotalHP { get; set; }

        public int CurHP { get; set; }

        public int TotalMP { get; set; }

        public int CurMP { get; set; }

        public string Name { get; set; }

        public long QQNum { get; set; }

        public long CurGroup { get; set; }

        public int Level { get; set; }

        public List<IMagic> Magics { get; set; }

        public Player(long QQNum)
        {
            this.QQNum = QQNum;
        }
    }
}
