﻿using System.Threading;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.SyntaxChecker;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Doremi.Ai.Repeater
{
    public class RepeatorAI : AIBase
    {
        public override string AIName { get; set; } = "随机复读";
        public override string Description { get; set; } = "AI for Repeating Random words.";
        public override AIPriority PriorityLevel { get; } = AIPriority.SuperLow;

        private const long RepeatLimit = 30;

        private long CurCount;

        private const int SleepTime = 2000;

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

            var atChecker = new AtChecker();
            if (atChecker.Check(MsgDTO.FullMsg, out _))
            {
                return false;
            }

            CurCount++;
            if (CurCount < RepeatLimit)
            {
                return false;
            }

            CurCount %= RepeatLimit;
            Repeat(MsgDTO);
            AIAnalyzer.AddCommandCount(new CmdRec()
            {
                FunctionalAi = AIName,
                Command = "RepeatorOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            return true;
        }

        private static void Repeat(MsgInformationEx MsgDTO)
        {
            Thread.Sleep(SleepTime);

            MsgSender.PushMsg(MsgDTO, MsgDTO.FullMsg);
        }
    }
}
