﻿using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.SingleCommand.PraiseMe
{
    [AI(Name = "点赞",
        Description = "AI for Praise someone.",
        Enable = true,
        PriorityLevel = 10)]
    public class PraiseMeAI : AIBase
    {
        private readonly Dictionary<string, DateTime> LastTimeDic = new Dictionary<string, DateTime>();

        private readonly int PraiseLimit = Configger.Instance.AIConfig.PraiseLimit;

        [EnterCommand(ID = "PraiseMeAI_PraiseMe",
            Command = "赞我",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送十个赞",
            Syntax = "",
            Tag = "娱乐功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            IsGroupAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool PraiseMe(MsgInformationEx MsgDTO, object[] param)
        {
            if (!CheckLimit(MsgDTO))
            {
                return false;
            }

            Praise(MsgDTO);
            LastTimeDic[MsgDTO.BindAi] = DateTime.Now;

            return true;
        }

        private bool CheckLimit(MsgInformationEx MsgDTO)
        {
            if (!LastTimeDic.ContainsKey(MsgDTO.BindAi))
            {
                LastTimeDic.Add(MsgDTO.BindAi, DateTime.Now);
                return true;
            }
            if (LastTimeDic[MsgDTO.BindAi].AddMinutes(PraiseLimit) < DateTime.Now)
            {
                return true;
            }

            var cd = LastTimeDic[MsgDTO.BindAi].AddMinutes(PraiseLimit) - DateTime.Now;
            MsgSender.PushMsg(MsgDTO, $"点赞太频繁啦！剩余冷却时间:{cd.Minutes}分{cd.Seconds}秒");
            return false;
        }

        private static void Praise(MsgInformationEx MsgDTO)
        {
            APIEx.SendPraise(MsgDTO.FromQQ, MsgDTO.BindAi);
            MsgSender.PushMsg(MsgDTO, "已赞十次！", true);
        }
    }
}
