using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

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
        [ImportMany("GroupEnterCommand")]
        public IEnumerable<Lazy<Action<GroupMsgDTO, object[]>, IGroupEnterCommandCapabilities>> AllAvailableGroupCommands;

        public HelperAI()
            : base()
        {
            this.ComposePartsSelf();
        }

        public override void Work()
        {
        }

        [GroupEnterCommandAttribute(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助信息",
            Syntax = " 或者 帮助 [命令名]",
            Tag = "帮助功能"
            )]
        public void HelpMe(GroupMsgDTO MsgDTO, object[] param)
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
            string helpMsg = "当前的命令标签有：";
            var commandAttrs = GetCommandAttrs();
            foreach (var c in commandAttrs)
            {
                helpMsg += '\r' + c.Tag;
            }

            helpMsg += '\r' + "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });
        }

        private IEnumerable<IGroupEnterCommandCapabilities> GetCommandAttrs()
        {
            return AllAvailableGroupCommands
                .GroupBy(c => c.Metadata.Tag)
                .Select(p => p.First().Metadata);
        }

        public bool HelpCommand(GroupMsgDTO MsgDTO)
        {
            var commands = AllAvailableGroupCommands.Where(c => c.Metadata.Command == MsgDTO.msg);
            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            var command = commands.FirstOrDefault();
            string helpMsg = $@"命令：{command.Metadata.Command}
格式： {command.Metadata.Command} {command.Metadata.Syntax}
描述： {command.Metadata.Description}
权限： {command.Metadata.AuthorityLevel.ToString()}";

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
            var commands = AllAvailableGroupCommands.Where(c => c.Metadata.Tag == MsgDTO.msg);
            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            string helpMsg = $@"当前标签下有以下命令：";
            foreach (var c in commands)
            {
                helpMsg += '\r' + c.Metadata.Command;
            }
            helpMsg += '\r' + "可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });

            return true;
        }
    }
}