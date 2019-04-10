using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Sys
{
    [AI(
        Name = "反馈",
        Description = "AI for Feedback.",
        Enable = true,
        PriorityLevel = 10)]
    public class FeedbackAi : AIBase
    {
        [EnterCommand(
            Command = "反馈",
            Description = "向开发者提供反馈建议",
            Syntax = "[反馈内容]",
            Tag = "系统命令",
            SyntaxChecker = "Any",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool Feedback(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;
            if (string.IsNullOrEmpty(content))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "反馈内容不能为空！");
                return false;
            }

            var feedback = new FeedbackRecord {GroupNum = MsgDTO.FromGroup, QQNum = MsgDTO.FromQQ, Content = content, UpdateTime = DateTime.Now};
            MongoService<FeedbackRecord>.Insert(feedback);

            MsgSender.Instance.PushMsg(MsgDTO, "感谢你的反馈，我会变得更强！");
            return true;
        }

        [EnterCommand(
            Command = "昨日反馈",
            Description = "查看昨日反馈",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ViewFeedback(MsgInformationEx MsgDTO, object[] param)
        {
            var expiryDate = DateTime.Now.AddDays(-7);
            MongoService<FeedbackRecord>.DeleteMany(p => p.UpdateTime <= expiryDate);

            var endTime = DateTime.Now.Date;
            var startTime = endTime.AddDays(-1);
            var records = MongoService<FeedbackRecord>.Get(p => p.UpdateTime >= startTime && p.UpdateTime < endTime);
            if (records.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "没有任何反馈内容！");
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, string.Join("    ", records.Select(p => p.Content)));
            return true;
        }
    }
}
