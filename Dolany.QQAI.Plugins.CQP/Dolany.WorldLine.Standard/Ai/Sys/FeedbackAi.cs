using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class FeedbackAi : AIBase
    {
        public override string AIName { get; set; } = "反馈";

        public override string Description { get; set; } = "AI for Feedback.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        [EnterCommand(ID = "FeedbackAi_Feedback",
            Command = "反馈",
            Description = "向开发者提供反馈建议",
            Syntax = "[反馈内容]",
            Tag = CmdTagEnum.系统命令,
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

            if ((content.Contains("啪") || content.Contains("口拍")) && content.Contains("师") && (content.Contains("姐") || content.Contains("女且")))
            {
                MsgSender.PushMsg(MsgDTO, "【哔哔，禁止事项！】");
                return false;
            }

            MsgSender.PushMsg(0, Global.DeveloperNumber, content, Global.DefaultConfig.MainAi);

            MsgSender.PushMsg(MsgDTO, "感谢你的反馈，我会变得更强！");
            return true;
        }
    }
}
