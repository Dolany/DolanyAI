using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Advanture
{
    public class AdvGameMgr
    {
        private readonly List<AdvGameEngine> Engines = new List<AdvGameEngine>();

        public static AdvGameMgr Instance { get; } = new AdvGameMgr();

        private AdvGameMgr()
        {

        }

        public bool CheckGroup(long groupNum)
        {
            return Engines.All(p => p.GroupNum != groupNum);
        }

        public bool CheckPlayer(long QQNum)
        {
            return Engines.All(p => p.players.All(player => player.QQNum != QQNum));
        }

        public void GameStart(long groupNum, long firstPlayer, long secondPlayer, int CaveNo)
        {
            var players = new[] {AdvPlayer.GetPlayer(firstPlayer), AdvPlayer.GetPlayer(secondPlayer)};
            players = CommonUtil.RandSort(players);

            var engine = new AdvGameEngine(players, groupNum, CaveNo);
            Engines.Add(engine);
            engine.GameStart();

            Engines.Remove(engine);
        }
    }
}
