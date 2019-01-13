namespace Dolany.Ai.Core.Ai.SingleCommand.PraiseMe
{
    using System;
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Database;
    using Dolany.Database.Ai;
    using Dolany.Database.Redis;
    using Dolany.Database.Redis.Model;

    using Model;

    using static Dolany.Ai.Core.API.APIEx;

    [AI(
        Name = nameof(PraiseMeAI),
        Description = "AI for Praise someone.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class PraiseMeAI : AIBase
    {
        private DateTime LastTime;

        private int PraiseLimit
        {
            get
            {
                var config = CommonUtil.GetConfig(nameof(PraiseLimit), "10");

                return int.Parse(config);
            }
        }

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

            var redisKey = $"PraiseRec-{MsgDTO.FromQQ}";
            var redisValue = CacheService.Get<PraiseRecCache>(redisKey);
            if (redisValue == null)
            {
                Praise(MsgDTO);
                CacheService.Insert(
                    redisKey,
                    new PraiseRecCache { QQNum = MsgDTO.FromQQ },
                    CommonUtil.UntilTommorow());
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
