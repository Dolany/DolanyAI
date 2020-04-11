using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst
{
    public class PetAgainstSvc : IDependency
    {
        private List<PetAgainstEngine> Engines { get; set; }

        public PetLevelSvc PetLevelSvc { get; set; }

        public PetAgainstSvc()
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
                    HP = PetLevelSvc[selfPet.Level].HP,
                    QQNum = selfPet.QQNum,
                    Skills = new Dictionary<string, int>(selfPet.Skills),
                    Level = selfPet.Level
                },
                AimPet = new GamingPet()
                {
                    Name = aimPet.Name,
                    HP = PetLevelSvc[aimPet.Level].HP,
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
