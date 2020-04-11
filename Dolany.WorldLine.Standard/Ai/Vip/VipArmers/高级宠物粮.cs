using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.Pet;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 高级宠物粮 : IVipArmer
    {
        public string Name { get; set; } = "高级宠物粮";
        public string Description { get; set; } = "宠物立刻获得100经验值，每日限购一次。";
        public int DiamondsNeed { get; set; } = 75;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Daily;
        public int LimitCount { get; set; } = 1;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var msg = pet.ExtGain(MsgDTO, 100);
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        public int MaxContains { get; set; }
    }
}
