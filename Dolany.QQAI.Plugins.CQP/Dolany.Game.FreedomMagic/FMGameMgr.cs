using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMGameMgr
    {
        private static List<FMEngine> Gamings = new List<FMEngine>();

        public static bool IsPlaying(long GroupNum, long FirstQQNum, long SecondQQNum)
        {
            return Gamings.Any(g => g.GroupNum == GroupNum || g.FirstPlayer.QQNum == FirstQQNum || g.SecondePlayer.QQNum == FirstQQNum ||
                                    g.FirstPlayer.QQNum == SecondQQNum || g.SecondePlayer.QQNum == SecondQQNum);
        }

        public void GameStart(long GroupNum, FMPlayerEx firstPlayer, FMPlayerEx SecondPlayer, Action<long, string> CommandCallBack,
            Func<MsgCommand, Predicate<MsgInformation>, int, MsgInformation> WaitCallBack)
        {
            var engine = new FMEngine(GroupNum, firstPlayer, SecondPlayer, CommandCallBack, WaitCallBack);
            Gamings.Add(engine);
            Task.Factory.StartNew(engine.GameStart);
        }
    }
}
