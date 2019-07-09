using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Sys
{
    public class OverHeatAI : AIBase
    {
        public override string AIName { get; set; } = "过热";

        public override string Description { get; set; } = "AI for monitor over heat.";

        public override int PriorityLevel { get; set; } = 50;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            return base.OnMsgReceived(MsgDTO) || RecentCommandCache.IsTooFreq(MsgDTO.BindAi);
        }
    }
}
