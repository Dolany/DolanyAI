using System.Collections.Generic;
using System.Linq;

namespace Dolany.Ai.Core.Ai.Game.Pet.PetAgainst
{
    public class PetAgainstMgr
    {
        public static PetAgainstMgr Instance { get; set; } = new PetAgainstMgr();

        private List<PetAgainstEngine> Engines { get; set; }

        private PetAgainstMgr()
        {
            Engines = new List<PetAgainstEngine>();
        }

        public bool CheckGroup(long GroupNum)
        {
            return Engines.All(p => p.GroupNum != GroupNum);
        }

        public bool CheckQQ(long QQNum)
        {
            return Engines.All(p => p.SelfPet.QQNum != QQNum && p.AimPet.QQNum != QQNum);
        }

        public void StartGame(PetRecord selfPet, PetRecord aimPet, long groupNum, string bindAi)
        {
            var engine = new PetAgainstEngine()
            {
                SelfPet = new GamingPet()
                {
                    Name = selfPet.Name,
                    HP = PetLevelMgr.Instance[selfPet.Level].HP,
                    QQNum = selfPet.QQNum,
                    Skills = new Dictionary<string, int>(selfPet.Skills),
                    Level = selfPet.Level
                },
                AimPet = new GamingPet()
                {
                    Name = aimPet.Name,
                    HP = PetLevelMgr.Instance[aimPet.Level].HP,
                    QQNum = aimPet.QQNum,
                    Skills = new Dictionary<string, int>(aimPet.Skills),
                    Level = aimPet.Level
                },
                GroupNum = groupNum,
                BindAi = bindAi
            };
            Engines.Add(engine);
            engine.StartGame();
            Engines.Remove(engine);
        }
    }
}
