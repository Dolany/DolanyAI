using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Sys
{
    using Base;

    using Model;

    [AI(
        Name = "系统消息过滤",
        Description = "AI for System msg consoling.",
        Enable = true,
        PriorityLevel = 20)]
    public class SysMsgAI : AIBase
    {
        public override void Initialization()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            return MsgDTO.FromQQ == Global.AnonymousNumber || MsgDTO.FromQQ == Global.SysMsgNumber;
        }
    }
}
