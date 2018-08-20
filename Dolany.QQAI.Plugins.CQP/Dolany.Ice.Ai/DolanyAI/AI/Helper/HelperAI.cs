using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(HelperAI),
        Description = "AI for Getting Help Infos.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class HelperAI : AIBase
    {
        public HelperAI()
            : base()
        {
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助信息",
            Syntax = " 或者 帮助 [命令名]",
            Tag = "帮助功能"
            )]
        public void HelpMe(GroupMsgDTO MsgDTO, object[] param)
        {
            if (string.IsNullOrEmpty(MsgDTO.Msg))
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

        public static void HelpSummary(GroupMsgDTO MsgDTO)
        {
            var helpMsg = "当前的命令标签有：";
            var commandAttrs = GetCommandAttrs();
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commandAttrs)
            {
                builder.Append('\r' + c.Tag);
            }
            helpMsg = builder.ToString();

            helpMsg += '\r' + "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });
        }

        private static IEnumerable<GroupEnterCommandAttribute> GetCommandAttrs()
        {
            return AIMgr.Instance.AllAvailableGroupCommands
                .GroupBy(c => c.Tag)
                .Select(p => p.First());
        }

        public static bool HelpCommand(GroupMsgDTO MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg);
            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            var command = commands.FirstOrDefault();
            var helpMsg = $@"命令：{command.Command}
格式： {command.Command} {command.Syntax}
描述： {command.Description}
权限： {command.AuthorityLevel.ToString()}";

            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });

            return true;
        }

        public static bool HelpTag(GroupMsgDTO MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands.Where(c => c.Tag == MsgDTO.Msg);
            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            var helpMsg = $@"当前标签下有以下命令：";
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commands)
            {
                builder.Append('\r' + c.Command);
            }
            helpMsg = builder.ToString();
            helpMsg += '\r' + "可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = helpMsg
            });

            return true;
        }
    }
}