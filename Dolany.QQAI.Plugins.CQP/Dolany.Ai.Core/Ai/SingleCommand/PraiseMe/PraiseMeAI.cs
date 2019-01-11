using Dolany.Database;

namespace Dolany.Ai.Core.Ai.SingleCommand.PraiseMe
{
    using System;
    using System.Linq;

    using Base;
    using Cache;
    using Common;
    using Model;
    using Dolany.Database.Ai;

    using static API.APIEx;
    using static Common.Utility;

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
                var config = GetConfig(nameof(PraiseLimit), "10");

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
            IsPrivateAvailable = true)]
        public void PraiseMe(MsgInformationEx MsgDTO, object[] param)
        {
            if (!CheckLimit(MsgDTO))
            {
                return;
            }

            var query = MongoService<PraiseRec>.Get(p => p.QQNum == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                LastTime = DateTime.Now;
                Praise(MsgDTO);
                MongoService<PraiseRec>.Insert(new PraiseRec
                {
                    Id = Guid.NewGuid().ToString(),
                    LastDate = DateTime.Now.Date,
                    QQNum = MsgDTO.FromQQ
                });
            }
            else if (query.First().LastDate >= DateTime.Now.Date)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "今天已经赞过十次啦！");
            }
            else
            {
                LastTime = DateTime.Now;
                Praise(MsgDTO);
                var praise = query.First();
                praise.LastDate = DateTime.Now.Date;

                MongoService<PraiseRec>.Update(praise);
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
