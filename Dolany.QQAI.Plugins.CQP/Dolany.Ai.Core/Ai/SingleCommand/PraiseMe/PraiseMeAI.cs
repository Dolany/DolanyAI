﻿namespace Dolany.Ai.Core.Ai.SingleCommand.PraiseMe
{
    using System;
    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Database.Sqlite;
    using Database.Sqlite.Model;

    using Model;

    using static API.APIEx;

    [AI(
        Name = nameof(PraiseMeAI),
        Description = "AI for Praise someone.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class PraiseMeAI : AIBase
    {
        private DateTime LastTime;

        private readonly int PraiseLimit = int.Parse(Configger.Instance["PraiseLimit"]);

        public PraiseMeAI()
        {
            RuntimeLogger.Log("PraiseMeAI started.");
        }

        public override void Work()
        {
            LastTime = DateTime.Now.AddMinutes(-PraiseLimit);
        }

        [EnterCommand(
            Command = "赞我",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "发送十个赞",
            Syntax = "",
            Tag = "点赞功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true,
            IsGroupAvailable = true)]
        public void PraiseMe(MsgInformationEx MsgDTO, object[] param)
        {
            if (!CheckLimit(MsgDTO))
            {
                return;
            }

            var key = $"PraiseRec-{MsgDTO.FromQQ}";
            var response = SqliteCacheService.Get<PraiseRecCache>(key);

            if (response == null)
            {
                Praise(MsgDTO);
                var model = new PraiseRecCache { QQNum = MsgDTO.FromQQ };
                SqliteCacheService.Cache(key, model, CommonUtil.UntilTommorow());
            }
            else
            {
                MsgSender.Instance.PushMsg(MsgDTO, "今天已经赞过十次啦！");
            }
        }

        private bool CheckLimit(MsgInformationEx MsgDTO)
        {
            if (LastTime.AddMinutes(PraiseLimit) < DateTime.Now)
            {
                return true;
            }

            var cdMinute = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Minutes;
            var cdSecond = (LastTime.AddMinutes(PraiseLimit) - DateTime.Now).Seconds;
            MsgSender.Instance.PushMsg(MsgDTO, $"点赞太频繁啦！剩余冷却时间:{cdMinute}分{cdSecond}秒");
            return false;
        }

        private static void Praise(MsgInformationEx MsgDTO)
        {
            SendPraise(MsgDTO.FromQQ);
            MsgSender.Instance.PushMsg(MsgDTO, "已赞十次！", true);
        }
    }
}
