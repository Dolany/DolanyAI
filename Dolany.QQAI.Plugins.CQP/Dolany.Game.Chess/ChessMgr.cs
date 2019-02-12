using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Game.Chess
{
    public class ChessMgr
    {
        public static ChessMgr Instance { get; } = new ChessMgr();

        private readonly List<ChessEngine> WorkingEngine = new List<ChessEngine>();

        private ChessMgr()
        {

        }

        public bool IsGroupInPlaying(long GroupNum)
        {
            return WorkingEngine.Any(e => e.GroupNum == GroupNum);
        }

        public bool IsQQInPlaying(long QQNum)
        {
            return WorkingEngine.Any(e => e.AimQQNum == QQNum || e.SelfQQNum == QQNum);
        }

        public void StartAGame(long GroupNum, long FirstQQ, long SecondQQ, Action<string, long, long> MsgCallBack, Func<long, long, string, Predicate<string>, string> WaitCallBack)
        {
            var ran = CommonUtil.RandInt(2);
            var SelfQQNum = ran == 0 ? FirstQQ : SecondQQ;
            var AimQQNum = ran == 1 ? FirstQQ : SecondQQ;

            var engine = new ChessEngine(GroupNum, SelfQQNum, AimQQNum, MsgCallBack, WaitCallBack);
            WorkingEngine.Add(engine);
            engine.GameStart();
        }
    }
}
