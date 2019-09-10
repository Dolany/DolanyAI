using System.Collections.Generic;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunYuanGamingModel
    {
        public long QQNum { get; set; }

        public Dictionary<string, int> Armers { get; set; }

        public Dictionary<string, int> EscapeArmers { get; set; }

        public int BasicHP { get; set; }

        public int HP { get; set; }

        public int BasicAttack { get; set; }

        public int Attack { get; set; }

        public bool IsDead => HP == 0;
    }
}
