using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.SegmentAttach;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 镀金权杖 : IVipArmer
    {
        public string Name { get; set; } = "镀金权杖";
        public string Description { get; set; } = "将你持有的宝藏碎片升级为稀有碎片！";
        public int DiamondsNeed { get; set; } = 150;
        public VipArmerLimitInterval LimitInterval { get; set; }
        public int LimitCount { get; set; }
        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var segment = SegmentRecord.Get(MsgDTO.FromQQ);
            if (string.IsNullOrEmpty(segment.Segment))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未持有任何宝藏碎片！");
                return false;
            }

            if (segment.IsRare)
            {
                MsgSender.PushMsg(MsgDTO, "你的宝藏碎片已经是稀有碎片了！");
                return false;
            }

            segment.IsRare = true;
            segment.Update();
            MsgSender.PushMsg(MsgDTO, "升级成功！");
            return true;
        }

        public int MaxContains { get; set; }
    }
}
