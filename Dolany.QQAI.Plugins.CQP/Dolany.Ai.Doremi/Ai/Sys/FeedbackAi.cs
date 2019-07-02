﻿using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "反馈",
        Description = "AI for Feedback.",
        Enable = true,
        PriorityLevel = 10,
        BindAi = "Doremi")]
    public class FeedbackAi : AIBase
    {
        [EnterCommand(ID = "FeedbackAi_Feedback",
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
                MsgSender.PushMsg(MsgDTO, "反馈内容不能为空！");
                return false;
            }

            if (content.Contains("啪") && content.Contains("师") && content.Contains("姐"))
            {
                MsgSender.PushMsg(MsgDTO, "哔哔，禁止事项！");
                return false;
            }

            MsgSender.PushMsg(0, Global.DeveloperNumber, content, Configger<AIConfigBase>.Instance.AIConfig.MainAi);

            MsgSender.PushMsg(MsgDTO, "感谢你的反馈，我会变得更强！");
            return true;
        }
    }
}