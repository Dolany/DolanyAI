﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.SyntaxChecker;
using Dolany.Database.Ai;
using Dolany.WorldLine.Doremi.Model;

namespace Dolany.WorldLine.Doremi.Ai.Repeater
{
    public class PlusOneAI : AIBase
    {
        public override string AIName { get; set; } = "+1复读";
        public override string Description { get; set; } = "AI for Auto Plus One.";
        public override AIPriority PriorityLevel { get; } = AIPriority.SuperLow;

        private List<PlusOneModel> Cache { get; } = new List<PlusOneModel>();

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

            var checker = new AtChecker();
            if (checker.Check(MsgDTO.FullMsg, out _))
            {
                return false;
            }

            var query = Cache.FirstOrDefault(d => d.GroupNumber == MsgDTO.FromGroup);
            if (query == null)
            {
                Cache.Add(new PlusOneModel
                {
                    GroupNumber = MsgDTO.FromGroup,
                    IsAlreadyRepeated = false,
                    MsgCache = MsgDTO.FullMsg
                });

                return false;
            }

            Thread.Sleep(2000);
            Repeat(MsgDTO, query);
            return true;
        }

        private void Repeat(MsgInformationEx MsgDTO, PlusOneModel groupCache)
        {
            if (groupCache.MsgCache != MsgDTO.FullMsg)
            {
                groupCache.MsgCache = MsgDTO.FullMsg;
                groupCache.IsAlreadyRepeated = false;

                return;
            }
            if (groupCache.IsAlreadyRepeated)
            {
                return;
            }

            AIAnalyzer.AddCommandCount(new CmdRec()
            {
                FunctionalAi = AIName,
                Command = "PlusOneOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            MsgSender.PushMsg(MsgDTO, MsgDTO.FullMsg);
            groupCache.IsAlreadyRepeated = true;
        }
    }
}
