using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class GroupStateFilterAI : AIBase
    {
        public override string AIName { get; set; } = "群组状态过滤器";
        public override string Description { get; set; } = "AI for filter group state.";
        public override int PriorityLevel { get; set; } = 25;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var groupSetting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (groupSetting == null)
            {
                return true;
            }

            return groupSetting.ExpiryTime == null || groupSetting.ExpiryTime < DateTime.Now;
        }
    }
}
