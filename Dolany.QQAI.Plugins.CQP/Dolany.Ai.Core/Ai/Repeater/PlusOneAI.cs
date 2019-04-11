﻿using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Repeater
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Base;

    using Cache;
    using Model;

    using SyntaxChecker;

    [AI(
        Name = "+1复读",
        Description = "AI for Auto Plus One.",
        Enable = true,
        PriorityLevel = 1,
        NeedManulOpen = true)]
    public class PlusOneAI : AIBase
    {
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

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (!setting.HasFunction("+1复读"))
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

            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = Attr.Name,
                Command = "PlusOneOverride",
                GroupNum = MsgDTO.FromGroup
            });
            MsgSender.PushMsg(MsgDTO, MsgDTO.FullMsg);
            groupCache.IsAlreadyRepeated = true;
        }
    }
}
