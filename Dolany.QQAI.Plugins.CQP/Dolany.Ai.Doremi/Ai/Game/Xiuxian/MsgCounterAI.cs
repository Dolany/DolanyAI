using System.Collections.Generic;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;

namespace Dolany.Ai.Doremi.Ai.Game.Xiuxian
{
    [AI(Name = "修仙计数器",
        Description = "AI for Msg Count for Xiuxian.",
        Enable = true,
        PriorityLevel = 15,
        NeedManulOpen = true,
        BindAi = "Doremi")]
    public class MsgCounterAI : AIBase
    {
        private List<long> EnablePersons = new List<long>();

        public override void Initialization()
        {
            EnablePersons = MsgCounterSvc.GetAllEnabledPersons();
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private || !EnablePersons.Contains(MsgDTO.FromQQ))
            {
                return false;
            }

            MsgCounterSvc.Cache(MsgDTO.FromQQ);
            return false;
        }
    }
}
