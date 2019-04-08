using System;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
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
    }
}
