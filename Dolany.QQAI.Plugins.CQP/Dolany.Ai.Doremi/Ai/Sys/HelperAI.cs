using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "帮助",
        Description = "AI for Getting Help Infos.",
        Enable = true,
        PriorityLevel = 10,
        BindAi = "Doremi")]
    public class HelperAI : AIBase
    {
        [EnterCommand(ID = "HelperAI_HelpMe",
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助列表",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public bool HelpMe(MsgInformationEx MsgDTO, object[] param)
        {
            HelpSummary(MsgDTO);
            return true;
        }

        [EnterCommand(ID = "HelperAI_HelpMe_Command",
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取特定命令或标签的帮助信息",
            Syntax = "[命令名/标签名]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public bool HelpMe_Command(MsgInformationEx MsgDTO, object[] param)
        {
            if (HelpCommand(MsgDTO))
            {
                return true;
            }

            HelpTag(MsgDTO);
            return true;
        }

        private void HelpSummary(MsgInformationEx MsgDTO)
        {
            var helpMsg = "当前的命令标签有：";
            var commandAttrs = AiSvc.AllAvailableGroupCommands.GroupBy(c => c.Tag)
                                                                       .Select(p => p.First());
            if (MsgDTO.Auth != AuthorityLevel.开发者)
            {
                commandAttrs = commandAttrs.Where(p => p.AuthorityLevel != AuthorityLevel.开发者);
            }
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commandAttrs)
            {
                builder.Append($"\r\n{c.Tag}");
            }
            helpMsg = builder.ToString();

            helpMsg += "\r\n可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
        }

        private bool HelpCommand(MsgInformationEx MsgDTO)
        {
            var commands = AiSvc.AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg).ToList();
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
                var range = new List<string>();
                if (command.IsGroupAvailable)
                {
                    range.Add("群组");
                }

                if (command.IsPrivateAvailable)
                {
                    range.Add("私聊");
                }

                var helpMsg = $"命令：{command.Command}\r\n" +
                              $"格式：{command.Command} {command.Syntax}\r\n" +
                              $"描述：{command.Description}\r\n" +
                              $"权限：{command.AuthorityLevel.ToString()}\r\n" +
                              $"适用范围：{string.Join("，", range)}\r\n" +
                              $"次数限制：{(Global.TestGroups.Contains(MsgDTO.FromGroup) ? command.TestingDailyLimit : command.DailyLimit)}";

                MsgSender.PushMsg(MsgDTO, helpMsg);
            }

            return true;
        }

        private void HelpTag(MsgInformationEx MsgDTO)
        {
            var commands = AiSvc.AllAvailableGroupCommands
                .Where(c => c.Tag == MsgDTO.Msg).GroupBy(p => p.Command)
                .Select(p => p.First()).ToList();
            if (!Global.TestGroups.Contains(MsgDTO.FromGroup))
            {
                commands = commands.Where(c => !c.IsTesting).ToList();
            }

            if (MsgDTO.Auth != AuthorityLevel.开发者)
            {
                commands = commands.Where(c => c.AuthorityLevel != AuthorityLevel.开发者).ToList();
            }

            if (commands.IsNullOrEmpty())
            {
                return;
            }

            var helpMsg = "当前标签下有以下命令：";
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            var groups = commands.GroupBy(p => p.ID);
            foreach (var group in groups)
            {
                builder.Append($"\r\n{string.Join("/", group.Select(g => g.Command))}");
            }
            helpMsg = builder.ToString();
            helpMsg += "\r\n可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
        }
    }
}
