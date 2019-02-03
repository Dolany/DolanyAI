using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMGameMgr
    {
        private static readonly List<FMEngine> Gamings = new List<FMEngine>();

        public static bool IsPlaying_Group(long GroupNum)
        {
            return Gamings.Any(g => g.GroupNum == GroupNum);
        }

        public static bool IsPlaying_Player(long QQNum)
        {
            return Gamings.Any(g => g.FirstPlayer.QQNum == QQNum || g.SecondePlayer.QQNum == QQNum);
        }

        public static void GameStart(long GroupNum, FMPlayerEx firstPlayer, FMPlayerEx SecondPlayer, Action<string, long> CommandCallBack,
            Func<string, Predicate<MsgInformation>, int, MsgInformation> WaitCallBack)
        {
            var engine = new FMEngine(GroupNum, firstPlayer, SecondPlayer, CommandCallBack, WaitCallBack);
            Gamings.Add(engine);
            Task.Factory.StartNew(engine.GameStart).ContinueWith(task => Gamings.Remove(engine));
        }
    }
}
