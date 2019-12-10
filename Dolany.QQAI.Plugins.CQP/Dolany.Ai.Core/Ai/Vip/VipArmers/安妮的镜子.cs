using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Vip.VipArmers
{
    public class 安妮的镜子 : IVipArmer
    {
        public string Name { get; set; } = "安妮的镜子";
        public string Description { get; set; } = "额外获得一个下次捞瓶子获得的物品的复制，最多同时持有1个，每天最多购买2次。";
        public int DiamondsNeed { get; set; } = 30;
        public VipArmerLimitInterval LimitInterval { get; set; }
        public int LimitCount { get; set; }

        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var armerRec = VipArmerRecord.Get(MsgDTO.FromQQ);
            var armer = new ArmerModel {Name = Name, Description = Description};
            armerRec.Armers.Add(armer);
            armerRec.Update();

            MsgSender.PushMsg(MsgDTO, "购买成功！");
            return true;
        }

        public int MaxContains { get; set; } = 1;
    }
}
