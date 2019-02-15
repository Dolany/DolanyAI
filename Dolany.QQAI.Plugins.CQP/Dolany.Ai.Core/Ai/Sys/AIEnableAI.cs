using System.Linq;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;

namespace Dolany.Ai.Core.Ai.Sys
{
    [AI(
        Name = "AI管理",
        Description = "AI for ais's state(open or closed).",
        Enable = true,
        PriorityLevel = 12)]
    public class AIEnableAI : AIBase
    {
        [EnterCommand(Command = "开启功能",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "开启某个AI功能",
            Syntax = "[功能名称]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = false)]
        public bool OpenFunction(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (!AIMgr.Instance.ManulOpenAiNames.Contains(name))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未查找到该功能名称，或者该功能无需手动开启！");
                return false;
            }

            AIStateMgr.Instance.AddSate(name, MsgDTO.FromGroup);
            MsgSender.Instance.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(Command = "关闭功能",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "关闭某个AI功能",
            Syntax = "[功能名称]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = false)]
        public bool CloseFunction(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (!AIMgr.Instance.ManulOpenAiNames.Contains(name))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未查找到该功能名称，或者该功能无需手动关闭！");
                return false;
            }

            AIStateMgr.Instance.RemoveSate(name, MsgDTO.FromGroup);
            MsgSender.Instance.PushMsg(MsgDTO, "关闭成功！");
            return true;
        }
    }
}
