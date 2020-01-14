using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.KindomStorm.Ai.Sys
{
    public class HelpAI : AIBase
    {
        public override string AIName { get; set; } = "帮助";

        public override string Description { get; set; } = "AI for Getting Help Infos.";

        public override int PriorityLevel { get; set; } = 10;

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

            if (HelpTag(MsgDTO))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, "很抱歉，未找到相关帮助信息！");
            return false;
        }

        private void HelpSummary(MsgInformationEx MsgDTO)
        {
            var commandAttrs = WorldLine.AllAvailableGroupCommands.GroupBy(c => c.Tag)
                                                                       .Select(p => p.First());
            if (MsgDTO.Auth != AuthorityLevel.开发者)
            {
                commandAttrs = commandAttrs.Where(p => p.AuthorityLevel != AuthorityLevel.开发者);
            }

            var helpMsg = string.Join("", commandAttrs.Select((command, idx) =>
                idx % 2 == 0 ? $"{Emoji.AllEmojis().RandElement()}{command.Tag}{Emoji.AllEmojis().RandElement()}" : $"{command.Tag}{Emoji.AllEmojis().RandElement()}\r"));

            helpMsg += '\r' + "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
        }

        private bool HelpCommand(MsgInformationEx MsgDTO)
        {
            var commands = WorldLine.AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg).ToList();
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

                var helpMsg = $"命令：{command.Command}\r" +
                              $"格式：{command.Command} {command.Syntax}\r" +
                              $"描述：{command.Description}\r" +
                              $"权限：{command.AuthorityLevel.ToString()}\r" +
                              $"适用范围：{string.Join("，", range)}\r" +
                              $"次数限制：{(Global.TestGroups.Contains(MsgDTO.FromGroup) ? command.TestingDailyLimit : command.DailyLimit)}";

                MsgSender.PushMsg(MsgDTO, helpMsg);
            }

            return true;
        }

        private bool HelpTag(MsgInformationEx MsgDTO)
        {
            var commands = WorldLine.AllAvailableGroupCommands
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
                return false;
            }

            var helpMsg = @"当前标签下有以下命令：";
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            var groups = commands.GroupBy(p => p.ID);
            foreach (var group in groups)
            {
                builder.Append('\r' + string.Join("/", group.Select(g => g.Command)));
            }
            helpMsg = builder.ToString();
            helpMsg += '\r' + "可以使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
            return true;
        }
    }
}
