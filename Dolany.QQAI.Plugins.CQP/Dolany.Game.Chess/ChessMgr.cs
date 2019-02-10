using System;
using System.Collections.Generic;
using System.Linq;

namespace Dolany.Game.Chess
{
    public class ChessMgr
    {
        public static ChessMgr Instance { get; } = new ChessMgr();

        private List<ChessEngine> WorkingEngine = new List<ChessEngine>();

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

        public void StartAGame(long GroupNum, long SelfQQNum, long AimQQNum, Action<string, long, long> MsgCallBack)
        {
            var engine = new ChessEngine(GroupNum, SelfQQNum, AimQQNum, MsgCallBack);
            WorkingEngine.Add(engine);
            engine.GameStart();
        }
    }
}
