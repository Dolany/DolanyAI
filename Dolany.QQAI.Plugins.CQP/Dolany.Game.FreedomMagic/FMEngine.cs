using System;
using Dolany.Ai.Common;

namespace Dolany.Game.FreedomMagic
{
    public class FMEngine
    {
        public long GroupNum { get; set; }

        public FMPlayerEx FirstPlayer { get; set; }

        public FMPlayerEx SecondePlayer { get; set; }

        private Action<string, long> CommandCallBack { get; set; }

        private Func<string, Predicate<MsgInformation>, int, MsgInformation> WaitCallBack;

        public FMEngine(long GroupNum, FMPlayerEx firstPlayer, FMPlayerEx SecondPlayer, Action<string, long> CommandCallBack,
            Func<string, Predicate<MsgInformation>, int, MsgInformation> WaitCallBack)
        {
            this.GroupNum = GroupNum;
            this.FirstPlayer = firstPlayer;
            this.SecondePlayer = SecondPlayer;
            this.CommandCallBack = CommandCallBack;
            this.WaitCallBack = WaitCallBack;
        }

        public void GameStart()
        {
            // todo
        }
    }
}
