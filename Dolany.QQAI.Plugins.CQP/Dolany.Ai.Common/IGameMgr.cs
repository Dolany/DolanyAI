using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Common
{
    public abstract class IGameMgr<EngineType, GamerModel> where GamerModel : IQQNumEntity where EngineType : IGameEngine<GamerModel>, new()
    {
        public List<EngineType> EngineList { get; set; }

        public string Name { get; set; }

        protected IGameMgr(string Name)
        {
            this.Name = Name;
            EngineList = new List<EngineType>();
        }

        public bool CheckGroup(long GroupNum)
        {
            return EngineList.All(p => p.GroupNum != GroupNum);
        }

        public bool CheckQQ(long QQNum)
        {
            return EngineList.All(p => p.Gamers.All(g => g.QQNum != QQNum));
        }

        public void StartGame(GamerModel[] Gamers, long groupNum, string bindAi)
        {
            var engine = new EngineType()
            {
                Gamers = Gamers,
                GroupNum = groupNum,
                BindAi = bindAi,
                Name = Name
            };
            EngineList.Add(engine);
            engine.StartGame();
            EngineList.Remove(engine);
        }
    }
}
