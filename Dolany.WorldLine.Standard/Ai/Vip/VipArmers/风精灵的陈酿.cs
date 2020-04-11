using System;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.Pet;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 风精灵的陈酿 : IVipArmer
    {
        public string Name { get; set; } = "风精灵的陈酿";
        public string Description { get; set; } = "恢复宠物一半的耐力(无法超过耐力上限)，每周限购2次。";
        public int DiamondsNeed { get; set; } = 150;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Weekly;
        public int LimitCount { get; set; } = 2;

        private static PetLevelSvc PetLevelSvc => AutofacSvc.Resolve<PetLevelSvc>();

        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var petLevel = PetLevelSvc[pet.Level];
            var enduranceConsume = PetEnduranceRecord.Get(MsgDTO.FromQQ);

            var restoreEndurance = Math.Min(petLevel.Endurance / 2, enduranceConsume.ConsumeTotal);
            enduranceConsume.ConsumeTotal -= restoreEndurance;
            enduranceConsume.Update();

            MsgSender.PushMsg(MsgDTO, $"恭喜{pet.Name}恢复了 {restoreEndurance}点耐力！");
            return true;
        }

        public int MaxContains { get; set; }
    }
}
