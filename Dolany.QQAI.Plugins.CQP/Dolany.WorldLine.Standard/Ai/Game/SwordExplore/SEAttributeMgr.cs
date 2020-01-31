using System.Collections.Generic;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SEAttributeMgr
    {
        public static SEAttributeMgr Instance { get; } = new SEAttributeMgr();

        public string[] Attrs = {"火焰", "寒冰", "雷电"};

        public Dictionary<string, string> StrongerThan = new Dictionary<string, string>()
        {
            {"火焰", "雷电" },
            {"寒冰", "火焰" },
            {"雷电", "寒冰" }
        };

        private SEAttributeMgr()
        {

        }
    }
}
