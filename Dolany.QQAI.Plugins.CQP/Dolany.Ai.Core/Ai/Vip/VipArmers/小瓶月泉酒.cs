using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Pet;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 小瓶月泉酒 : IVipArmer
    {
        public string Name { get; set; } = "小瓶月泉酒";
        public string Description { get; set; } = "恢复10点耐力值，每天最多购买一次。";
        public int DiamondsNeed { get; set; } = 20;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Daily;
        public int LimitCount { get; set; } = 1;

        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var endurance = PetEnduranceRecord.Get(MsgDTO.FromQQ);
            endurance.ConsumeTotal -= 10;
            endurance.ConsumeTotal = Math.Max(0, endurance.ConsumeTotal);
            endurance.Update();

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, $"{pet.Name}感觉一股清凉传遍全身，恢复了10点耐力！");
            return true;
        }

        public int MaxContains { get; set; }
    }
}
