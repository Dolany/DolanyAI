using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.ChessAgainst
{
    public class ChessSvc : IDependency
    {
        private readonly List<ChessEngine> WorkingEngine = new List<ChessEngine>();

        public bool IsGroupInPlaying(long GroupNum)
        {
            return WorkingEngine.Any(e => e.GroupNum == GroupNum);
        }

        public bool IsQQInPlaying(long QQNum)
        {
            return WorkingEngine.Any(e => e.AimQQNum == QQNum || e.SelfQQNum == QQNum);
        }

        public void StartAGame(long GroupNum, long FirstQQ, long SecondQQ, Func<long, long, string, Predicate<string>, string> WaitCallBack, string BindAi)
        {
            var ran = Rander.RandInt(2);
            var SelfQQNum = ran == 0 ? FirstQQ : SecondQQ;
            var AimQQNum = ran == 1 ? FirstQQ : SecondQQ;

            var engine = new ChessEngine(GroupNum, SelfQQNum, AimQQNum, WaitCallBack, BindAi);
            WorkingEngine.Add(engine);
            engine.GameStart();
        }

        public void GameOver(ChessEngine engine)
        {
            WorkingEngine.Remove(engine);
        }
    }
}
