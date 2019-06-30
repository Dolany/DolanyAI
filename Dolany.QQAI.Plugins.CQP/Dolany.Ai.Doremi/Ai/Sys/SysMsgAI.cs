using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Common;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "系统消息过滤",
        Description = "AI for System msg consoling.",
        Enable = true,
        PriorityLevel = 20,
        BindAi = "Doremi")]
    public class SysMsgAI : AIBase
    {
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
