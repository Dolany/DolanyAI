using System.Collections.Generic;
using System.Linq;
using Dolany.Game.MagicCleanUp.Magic;

namespace Dolany.Game.MagicCleanUp
{
    public class GameMgr
    {
        public static GameMgr Instance { get; } = new GameMgr();
        private GameMgr()
        {
        }
    }
}
