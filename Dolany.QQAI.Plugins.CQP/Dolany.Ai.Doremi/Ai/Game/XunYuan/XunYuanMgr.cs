using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunYuanMgr : IDependency
    {
        private readonly List<XunyuanEngine> EngineList = new List<XunyuanEngine>();

        public bool CheckGroup(long GroupNum)
        {
            return EngineList.All(e => e.GroupNum != GroupNum);
        }

        public bool CheckQQNum(long QQNum)
        {
            return EngineList.All(e => e.Gamers.All(g => g.QQNum != QQNum));
        }

        public void StartGame(XunYuanGamingModel[] Gamers, long GroupNum, string BindAi)
        {
            var engine = new XunyuanEngine(Gamers, GroupNum, BindAi);
            EngineList.Add(engine);
            engine.StartGame();

            EngineList.Remove(engine);
        }
    }
}
