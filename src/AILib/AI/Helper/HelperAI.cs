using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "HelperAI",
        Description = "AI for Getting Help Infos.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class HelperAI : AIBase
    {
        public HelperAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(Command = "帮助", SourceType = MsgType.Group, AuthorityLevel = AuthorityLevel.成员)]
        public void HelpMe(GroupMsgDTO MsgDTO)
        {
            if (string.IsNullOrEmpty(MsgDTO.msg))
            {
                HelpSummary(MsgDTO);
                return;
            }

            HelpCommand(MsgDTO);
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

        public void HelpCommand(GroupMsgDTO MsgDTO)
        {
            var commands = AIMgr.AllAvailableCommands.Where(c => c.Command == MsgDTO.msg && c.SourceType == MsgType.Group);
            if (commands == null || commands.Count() == 0)
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = "哎呀呀！人家还不知道这条命令呢！"
                });
                return;
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
        }
    }
}