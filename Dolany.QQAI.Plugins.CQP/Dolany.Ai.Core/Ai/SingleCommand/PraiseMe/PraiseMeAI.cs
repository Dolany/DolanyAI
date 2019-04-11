namespace Dolany.Ai.Core.Ai.SingleCommand.PraiseMe
{
    using System;
    using Base;

    using Cache;
    using Dolany.Ai.Common;
    using Model;

    using static API.APIEx;

    [AI(
        Name = "点赞",
        Description = "AI for Praise someone.",
        Enable = true,
        PriorityLevel = 10)]
    public class PraiseMeAI : AIBase
    {
        private DateTime LastTime;

        private readonly int PraiseLimit = int.Parse(Configger.Instance["PraiseLimit"]);

        public override void Initialization()
        {
            LastTime = DateTime.Now.AddMinutes(-PraiseLimit);
        }

        [EnterCommand(
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
            LastTime = DateTime.Now;

            return true;
        }

        private bool CheckLimit(MsgInformationEx MsgDTO)
        {
            if (LastTime.AddMinutes(PraiseLimit) < DateTime.Now)
            {
                return true;
            }

            var cd = LastTime.AddMinutes(PraiseLimit) - DateTime.Now;
            MsgSender.PushMsg(MsgDTO, $"点赞太频繁啦！剩余冷却时间:{cd.Minutes}分{cd.Seconds}秒");
            return false;
        }

        private static void Praise(MsgInformationEx MsgDTO)
        {
            SendPraise(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, "已赞十次！", true);
        }
    }
}
