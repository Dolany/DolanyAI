using System;
using System.Collections.Generic;
using System.Threading;
using Dolany.Ai.Common;

namespace Dolany.Game.Engine
{
    public class GameEngine
    {
        private int MsgSpeed = 4;

        private readonly Action<MsgCommand> CommandCallBack;
        private Func<MsgCommand, Predicate<MsgInformation>, int, MsgInformation> WaitForInformation;
        private Func<MsgCommand, IEnumerable<Predicate<MsgInformation>>, int, IEnumerable<MsgInformation>> WaitForInformations;
        private Func<MsgCommand, Predicate<MsgInformation>, int, List<MsgInformation>> WaitWhile;

        private ScriptModel Model;

        private long Gamer;

        public GameEngine(Action<MsgCommand> CommandCallBack, Func<MsgCommand, Predicate<MsgInformation>, int, MsgInformation> WaitForInformation,
            Func<MsgCommand, IEnumerable<Predicate<MsgInformation>>, int, IEnumerable<MsgInformation>> WaitForInformations,
            Func<MsgCommand, Predicate<MsgInformation>, int, List<MsgInformation>> WaitWhile, long Gamer)
        {
            this.CommandCallBack = CommandCallBack;
            this.WaitForInformation = WaitForInformation;
            this.WaitForInformations = WaitForInformations;
            this.WaitWhile = WaitWhile;
            this.Gamer = Gamer;
        }

        public void LoadGameScript(string scriptName)
        {
            Model = new ScriptModel(scriptName, OnErrorOccured);
        }

        public void GameStart(Dictionary<string, object> DataDic = null)
        {
            if (DataDic != null)
            {
                Model.RestoreData(DataDic);
            }

            // todo
        }

        public void SendMsg(string msg)
        {
            CommandCallBack(new MsgCommand{Command = AiCommand.SendPrivate, Msg = msg, ToQQ = Gamer});
            Thread.Sleep(msg.Length * 1000 / MsgSpeed);
        }

        private void OnErrorOccured(string msg)
        {
            SendMsg(msg);
            GameOver();
        }

        private void GameOver()
        {
            // todo
        }
    }
}
