using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetAgainstMgr
    {
        public static PetAgainstMgr Instance { get; set; }

        private List<PetAgainstEngine> Engines { get; set; } = new List<PetAgainstEngine>();

        private PetAgainstMgr(){}

        public bool CheckGroup(long GroupNum)
        {
            return Engines.All(p => p.GroupNum != GroupNum);
        }

        public bool CheckQQ(long QQNum)
        {
            return Engines.All(p => p.SelfPet.QQNum != QQNum && p.AimPet.QQNum != QQNum);
        }

        public void StartGame(GamingPet selfPet, GamingPet aimPet, long groupNum, string bindAi)
        {
            Engines.Add(new PetAgainstEngine()
            {
                SelfPet = selfPet,
                AimPet = aimPet,
                GroupNum = groupNum,
                BindAi = bindAi
            });
        }
    }
}
