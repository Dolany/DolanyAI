using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(
        Name = "HelperAI",
        Description = "AI for Getting Help Infos.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class HelperAI : AIBase
    {
        public HelperAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "帮助",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助信息",
            Syntax = " 或者 帮助 [命令名]",
            Tag = "帮助"
            )]
        public void HelpMe(GroupMsgDTO MsgDTO)
        {
            if (string.IsNullOrEmpty(MsgDTO.msg))
            {
                HelpSummary(MsgDTO);
                return;
            }

            if (HelpCommand(MsgDTO))
            {
                return;
            }

            if (HelpTag(MsgDTO))
            {
                return;
            }
        }

        public void HelpSummary(GroupMsgDTO MsgDTO)
        {
            string helpMsg = "当前可用命令有：";
            var commandAttrs = AIMgr.AllAvailableCommands.Where(c => c.SourceType == MsgType.Group);
            foreach (var c in commandAttrs)
            {
                helpMsg += '\r' + c.Command;
            }

            helpMsg += '\r' + "可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });
        }

        public bool HelpCommand(GroupMsgDTO MsgDTO)
        {
            var commands = AIMgr.AllAvailableCommands.Where(c => c.Command == MsgDTO.msg && c.SourceType == MsgType.Group);
            if (commands == null || commands.Count() == 0)
            {
                return false;
            }

            var command = commands.FirstOrDefault();
            string helpMsg = $@"命令：{command.Command}
格式： {command.Command} {command.Syntax}
描述： {command.Description}
权限： {command.AuthorityLevel.ToString()}";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });

            return true;
        }

        public bool HelpTag(GroupMsgDTO MsgDTO)
        {
            // TODO
            return false;
        }
    }
}