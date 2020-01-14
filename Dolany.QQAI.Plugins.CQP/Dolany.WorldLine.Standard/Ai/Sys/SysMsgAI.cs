using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class SysMsgAI : AIBase
    {
        public override string AIName { get; set; } = "系统消息过滤";

        public override string Description { get; set; } = "AI for System msg consoling.";

        public override int PriorityLevel { get; set; } = 20;

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
