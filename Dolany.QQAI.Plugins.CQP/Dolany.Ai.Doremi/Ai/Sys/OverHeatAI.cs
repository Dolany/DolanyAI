using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Common;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "过热",
        Description = "AI for monitor over heat.",
        Enable = true,
        PriorityLevel = 50,
        BindAi = "Doremi")]
    public class OverHeatAI : AIBase
    {
        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            return base.OnMsgReceived(MsgDTO) || RecentCommandCache.IsTooFreq(MsgDTO.BindAi);
        }
    }
}
