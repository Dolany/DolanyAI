using System.Collections.Generic;
using System.Linq;
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

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        [EnterCommand(ID = "HelperAI_HelpMe",
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取帮助列表",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = CmdTagEnum.系统命令,
            IsPrivateAvailable = true)]
        public bool HelpMe(MsgInformationEx MsgDTO, object[] param)
        {
            var helpMsg = "当前版本的命令标签有：\r\n";
            var tags = WorldLine.CmdTagTree.SubTags;

            helpMsg += string.Join("", tags.Select((tag, idx) =>
                idx % 2 == 0 ? $"{Emoji.AllEmojis().RandElement()}{tag.Tag}{Emoji.AllEmojis().RandElement()}" : $"{tag.Tag}{Emoji.AllEmojis().RandElement()}\r\n"));

            helpMsg += "\r\n可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
            return true;
        }

        [EnterCommand(ID = "HelperAI_HelpMe_Command",
            Command = "帮助",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取特定命令或标签的帮助信息",
            Syntax = "[命令名/标签名]",
            SyntaxChecker = "Word",
            Tag = CmdTagEnum.系统命令,
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

                var msgDic = new Dictionary<string, string>()
                {
                    {"命令", command.Command },
                    {"格式", $"{command.Command} {command.Syntax}" },
                    {"描述", command.Description },
                    {"权限", command.AuthorityLevel.ToString() },
                    {"标签", string.Join("-", WorldLine.LocateCmdPath(command).Select(p => p.Tag.ToString())) },
                    {"适用范围", string.Join("，", range) },
                    {"次数限制", Global.TestGroups.Contains(MsgDTO.FromGroup) ? command.TestingDailyLimit.ToString() : command.DailyLimit.ToString() }
                };

                var helpMsg = string.Join("\r\n", msgDic.Select(p => $"{p.Key}：{p.Value}"));

                MsgSender.PushMsg(MsgDTO, helpMsg);
            }

            return true;
        }

        private bool HelpTag(MsgInformationEx MsgDTO)
        {
            var tagName = MsgDTO.Msg;
            var tag = WorldLine.CmdTagTree.AllSubTags.FirstOrDefault(p => p.Tag.ToString() == tagName);
            if (tag == null)
            {
                return false;
            }

            var helpMsg = $"【{tagName}】\r\n";
            if (!tag.SubTags.IsNullOrEmpty())
            {
                helpMsg += "当前标签下的子标签有：\r\n";
                helpMsg += string.Join("",
                    tag.SubTags.Select((t, idx) =>
                        $"{(idx % 2 == 0 ? Emoji.AllEmojis().RandElement() : string.Empty)}{t.Tag}{Emoji.AllEmojis().RandElement()}{(idx % 2 == 0 ? "\r\n" : string.Empty)}"));
            }

            if (!tag.SubCmds.IsNullOrEmpty())
            {
                helpMsg += "当前标签下的命令有：\r\n";

                var groups = tag.SubCmds.GroupBy(p => p.ID);
                var subCommands = groups.Select(group => string.Join("/", group.Select(g => g.Command))).ToList();
                helpMsg += string.Join("",
                    subCommands.Select((command, idx) =>
                        $"{(idx % 2 == 0 ? Emoji.AllEmojis().RandElement() : string.Empty)}{command}{Emoji.AllEmojis().RandElement()}{(idx % 2 == 0 ? "\r\n" : string.Empty)}"));
            }

            helpMsg += "\r\n可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
            return true;
        }
    }
}
