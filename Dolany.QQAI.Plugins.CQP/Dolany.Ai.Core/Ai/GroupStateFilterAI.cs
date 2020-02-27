using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai
{
    public class GroupStateFilterAI : AIBase
    {
        public override string AIName { get; set; } = "群组状态过滤器";
        public override string Description { get; set; } = "AI for filter group state.";
        public override AIPriority PriorityLevel { get;} = AIPriority.System;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var groupSetting = GroupSettingSvc[MsgDTO.FromGroup];
            if (groupSetting == null)
            {
                return true;
            }

            return groupSetting.ExpiryTime == null || groupSetting.ExpiryTime < DateTime.Now;
        }
    }
}
