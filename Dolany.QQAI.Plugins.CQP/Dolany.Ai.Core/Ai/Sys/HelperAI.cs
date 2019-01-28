namespace Dolany.Ai.Core.Ai.Sys
{
    using System.Linq;
    using System.Text;

    using Core;
    using Base;
    using Cache;
    using Common;

    using Dolany.Ai.Common;

    using Model;

    [AI(
        Name = nameof(HelperAI),
        Description = "AI for Getting Help Infos.",
        Enable = true,
        PriorityLevel = 10)]
    public class HelperAI : AIBase
    {
        public override void Initialization()
        {
        }

        [EnterCommand(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助列表",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public void HelpMe(MsgInformationEx MsgDTO, object[] param)
        {
            HelpSummary(MsgDTO);
        }

        [EnterCommand(
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取特定命令或标签的帮助信息",
            Syntax = "[命令名/标签名]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public void HelpMe_Command(MsgInformationEx MsgDTO, object[] param)
        {
            if (HelpCommand(MsgDTO))
            {
                return;
            }

            HelpTag(MsgDTO);
        }

        private static void HelpSummary(MsgInformationEx MsgDTO)
        {
            var helpMsg = "当前的命令标签有：";
            var commandAttrs = AIMgr.Instance.AllAvailableGroupCommands.Where(p => p.AuthorityLevel != AuthorityLevel.开发者)
                                                                       .GroupBy(c => c.Tag)
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

        private static bool HelpCommand(MsgInformationEx MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg).ToList();
            if (!Global.TestGroups.Contains(MsgDTO.FromGroup))
            {
                commands = commands.Where(c => !c.IsTesting).ToList();
            }

            if (commands.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var command in commands)
            {
                var helpMsg = MsgDTO.Type == MsgType.Group ? $@"命令：{command.Command}
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

        private static void HelpTag(MsgInformationEx MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands
                .Where(c => c.Tag == MsgDTO.Msg && c.AuthorityLevel != AuthorityLevel.开发者).GroupBy(p => p.Command)
                .Select(p => p.First()).ToList();
            if (!Global.TestGroups.Contains(MsgDTO.FromGroup))
            {
                commands = commands.Where(c => !c.IsTesting).ToList();
            }

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
