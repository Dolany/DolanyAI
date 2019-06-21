using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Sys
{
    [AI(Name = "过热",
        Description = "AI for monitor over heat.",
        Enable = true,
        PriorityLevel = 50)]
    public class OverHeatAI : AIBase
    {
        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            return base.OnMsgReceived(MsgDTO) || RecentCommandCache.IsTooFreq(MsgDTO.BindAi);
        }
    }
}
