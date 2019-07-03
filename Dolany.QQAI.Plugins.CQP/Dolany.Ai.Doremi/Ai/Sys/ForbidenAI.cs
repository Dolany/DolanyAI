using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.API;
using Dolany.Ai.Doremi.Base;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "口球",
        Description = "AI for Silence.",
        Enable = true,
        PriorityLevel = 11,
        BindAi = "Doremi")]
    public class ForbidenAI : AIBase
    {
        private readonly List<FBModel> Models = new List<FBModel>();

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            var model = Models.FirstOrDefault(p => p.GroupNum == MsgDTO.FromGroup);
            if (model == null)
            {
                model = new FBModel
                {
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ,
                    Count = 0
                };
                Models.Add(model);
            }

            if (model.QQNum != MsgDTO.FromQQ)
            {
                model.QQNum = MsgDTO.FromQQ;
                model.Count = 1;
            }
            else
            {
                model.Count++;
            }

            if (model.Count < 5)
            {
                return false;
            }

            APIEx.Silence(model.GroupNum, model.QQNum, 5, MsgDTO.BindAi);
            return true;
        }
    }

    public class FBModel
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public int Count { get; set; }
    }
}
