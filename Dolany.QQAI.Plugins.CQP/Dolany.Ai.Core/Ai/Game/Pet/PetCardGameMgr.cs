using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetCardGameMgr
    {
        public static PetCardGameMgr Instance { get; } = new PetCardGameMgr();

        private readonly List<PetCardGameEngine> Engines = new List<PetCardGameEngine>();

        private PetCardGameMgr()
        {

        }

        public bool CheckQQ(long QQNum)
        {
            return Engines.All(e => e.GamingPets.All(p => p.QQNum != QQNum));
        }

        public bool CheckGroup(long GroupNum)
        {
            return Engines.All(e => e.GroupNum != GroupNum);
        }

        public void StartGame(PetCardRecord pet1, PetCardRecord pet2, long GroupNum, string bindAi)
        {
            var engine = new PetCardGameEngine(new []{pet1, pet2}, bindAi, GroupNum);
            Engines.Add(engine);
            engine.StartGame();

            Engines.Remove(engine);
        }
    }
}
