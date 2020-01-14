using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class AIEnableAI : AIBase
    {
        public override string AIName { get; set; } = "AI管理";

        public override string Description { get; set; } = "AI for ais's state(open or closed).";

        public override int PriorityLevel { get; set; } = 12;

        [EnterCommand(ID = "AIEnableAI_OpenFunction",
            Command = "开启功能",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "开启某个AI功能",
            Syntax = "[功能名称]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = false)]
        public bool OpenFunction(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (!WorldLine.ManulOpenAiNames.Contains(name))
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该功能名称，或者该功能无需手动开启！");
                return false;
            }

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            setting.EnabledFunctions.Add(name);
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(ID = "AIEnableAI_CloseFunction",
            Command = "关闭功能",
            AuthorityLevel = AuthorityLevel.群主,
            Description = "关闭某个AI功能",
            Syntax = "[功能名称]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = false)]
        public bool CloseFunction(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (!WorldLine.ManulOpenAiNames.Contains(name))
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该功能名称，或者该功能无需手动关闭！");
                return false;
            }

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            setting.EnabledFunctions.Remove(name);
            setting.Update();

            MsgSender.PushMsg(MsgDTO, "关闭成功！");
            return true;
        }
    }
}
