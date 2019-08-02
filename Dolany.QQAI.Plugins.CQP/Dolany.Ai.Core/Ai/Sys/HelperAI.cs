using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Ai.Game.Gift;
using Dolany.Ai.Core.Ai.Game.Pet;
using Dolany.Ai.Core.Ai.Game.SegmentAttach;
using Dolany.Ai.Core.Ai.Record;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Sys
{
    public class HelperAI : AIBase
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

            HelpTag(MsgDTO);
            return true;
        }

        private static void HelpSummary(MsgInformationEx MsgDTO)
        {
            var helpMsg = "当前的命令标签有：";
            var commandAttrs = AIMgr.Instance.AllAvailableGroupCommands.GroupBy(c => c.Tag)
                                                                       .Select(p => p.First());
            if (MsgDTO.Auth != AuthorityLevel.开发者)
            {
                commandAttrs = commandAttrs.Where(p => p.AuthorityLevel != AuthorityLevel.开发者);
            }
            var builder = new StringBuilder();
            builder.Append(helpMsg);
            foreach (var c in commandAttrs)
            {
                builder.Append('\r' + c.Tag);
            }
            helpMsg = builder.ToString();

            helpMsg += '\r' + "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
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

        private static void HelpTag(MsgInformationEx MsgDTO)
        {
            var commands = AIMgr.Instance.AllAvailableGroupCommands
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
        }

        [EnterCommand(ID = "HelperAI_ViewSomething",
            Command = "查看",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某个东西（物品/成就/礼物等等）",
            Syntax = "[名称]",
            SyntaxChecker = "Word",
            Tag = "系统命令",
            IsPrivateAvailable = true)]
        public bool ViewSomething(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (HonorHelper.Instance.FindItem(name) != null)
            {
                return AIMgr.Instance.AIInstance<DriftBottleAI>().ViewItem(MsgDTO, param);
            }

            if (HonorHelper.Instance.FindHonor(name) != null)
            {
                return AIMgr.Instance.AIInstance<DriftBottleAI>().ViewHonor(MsgDTO, param);
            }

            if (GiftMgr.Instance[name] != null)
            {
                return AIMgr.Instance.AIInstance<GiftAI>().ViewGift(MsgDTO, param);
            }

            if (PetSkillMgr.Instance[name] != null)
            {
                return AIMgr.Instance.AIInstance<PetAI>().ViewPetSkill(MsgDTO, param);
            }

            if (SegmentMgr.Instance.FindSegmentByName(name) != null)
            {
                return AIMgr.Instance.AIInstance<SegmentAttachAI>().ViewSegment(MsgDTO, param);
            }

            if (SegmentMgr.Instance.FindTreasureByName(name) != null)
            {
                return AIMgr.Instance.AIInstance<SegmentAttachAI>().ViewTreasure(MsgDTO, param);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关信息！");
            return false;
        }
    }
}
