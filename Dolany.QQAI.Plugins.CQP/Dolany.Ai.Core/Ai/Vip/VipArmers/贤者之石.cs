using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 贤者之石 : IVipArmer
    {
        public string Name { get; set; } = "贤者之石";
        public string Description { get; set; } = "使用钻石兑换1w金币，每周限购一次。";
        public int DiamondsNeed { get; set; } = 400;
        public VipArmerLimitInterval LimitInterval { get; set; } = VipArmerLimitInterval.Weekly;
        public int LimitCount { get; set; } = 1;
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            osPerson.Golds += 10000;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, $"兑换成功，你当前持有的金币为：{osPerson.Golds.CurencyFormat()}");
            return true;
        }

        public int MaxContains { get; set; }
    }
}
