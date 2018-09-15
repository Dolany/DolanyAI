using System.Linq;
using System.Text;

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
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助列表",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "帮助功能",
            IsPrivateAvailabe = true
            )]
        public void HelpMe(ReceivedMsgDTO MsgDTO, object[] param)
        {
            HelpSummary(MsgDTO);
        }

        [EnterCommand(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取特定命令或标签的帮助信息",
            Syntax = "[命令名/标签名]",
            SyntaxChecker = "NotEmpty",
            Tag = "帮助功能",
            IsPrivateAvailabe = true
        )]
        public void HelpMe_Command(ReceivedMsgDTO MsgDTO, object[] param)
        {
            if (HelpCommand(MsgDTO))
            {
                return;
            }

            HelpTag(MsgDTO);
        }

        private static void HelpSummary(ReceivedMsgDTO MsgDTO)
        {
            var helpMsg = "当前的命令标签有：";
            var commandAttrs = AIMgr.Instance.AllAvailableGroupCommands.GroupBy(c => c.Tag)
                                                                       .Select(p => p.First());
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commandAttrs)
            {
                builder.Append('\r' + c.Tag);
            }
            helpMsg = builder.ToString();

            helpMsg += '\r' + "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(MsgDTO, helpMsg);
        }

        private static bool HelpCommand(ReceivedMsgDTO MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg);
            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var command in commands)
            {
                var helpMsg = MsgDTO.MsgType == MsgType.Group ? $@"命令：{command.Command}
格式： {command.Command} {command.Syntax}
描述： {command.Description}
权限： {command.AuthorityLevel.ToString()}" :
                $@"命令：{command.Command}
格式： {command.Command} {command.Syntax}
描述： {command.Description}";

                MsgSender.Instance.PushMsg(MsgDTO, helpMsg);
            }

            return true;
        }

        private static void HelpTag(ReceivedMsgDTO MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands.Where(c => c.Tag == MsgDTO.Msg)
                                                                   .GroupBy(p => p.Command)
                                                                   .Select(p => p.First());
            if (commands.IsNullOrEmpty())
            {
                return;
            }

            var helpMsg = @"当前标签下有以下命令：";
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commands)
            {
                builder.Append('\r' + c.Command);
            }
            helpMsg = builder.ToString();
            helpMsg += '\r' + "可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.Instance.PushMsg(MsgDTO, helpMsg);
        }
    }
}