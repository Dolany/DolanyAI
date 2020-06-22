using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;
using Dolany.WorldLine.Standard.Ai.Game.DriftBottle;
using Dolany.WorldLine.Standard.Ai.Game.Gift;
using Dolany.WorldLine.Standard.Ai.Game.Pet;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition;
using Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst;
using Dolany.WorldLine.Standard.Ai.Game.SegmentAttach;
using Dolany.WorldLine.Standard.Ai.Game.TouhouCard;
using Dolany.WorldLine.Standard.Ai.Sys.Version;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Sys
{
    public class HelperAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "帮助";

        public override string Description { get; set; } = "AI for Getting Help Infos.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.帮助系统;

        private List<ExtraHelpModel> ExtraHelps = new List<ExtraHelpModel>();

        public DailyVipShopSvc DailyVipShopSvc { get; set; }
        public GiftSvc GiftSvc { get; set; }
        public PetSkillSvc PetSkillSvc { get; set; }
        public SegmentSvc SegmentSvc { get; set; }
        public CookingDietSvc CookingDietSvc { get; set; }
        public ExpeditionSceneSvc ExpeditionSceneSvc { get; set; }
        public HonorSvc HonorSvc { get; set; }
        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }
        public TouhouCardSvc TouhouCardSvc { get; set; }

        public void RefreshData()
        {
            ExtraHelps = CommonUtil.ReadJsonData_NamedList<ExtraHelpModel>("Standard/ExtraHelpData");
        }

        [EnterCommand(ID = "HelperAI_HelpMe",
            Command = "帮助",
            Description = "获取帮助列表",
            IsPrivateAvailable = true)]
        public bool HelpMe(MsgInformationEx MsgDTO, object[] param)
        {
            var versionAi = CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<VersionAi>();
            var version = versionAi.Versions.First();
            var helpMsg = $"当前版本为：{version.VersionNum}，请使用 【版本信息】 命令获取当前版本的更新内容！\r\n当前版本的命令标签有：\r\n";
            var tags = CrossWorldAiSvc[MsgDTO.FromGroup].CmdTagTree.SubTags;

            helpMsg += string.Join("", tags.Select((tag, idx) =>
                idx % 2 == 0 ? $"{Emoji.AllEmojis().RandElement()}{tag.Tag}{Emoji.AllEmojis().RandElement()}" : $"{tag.Tag}{Emoji.AllEmojis().RandElement()}\r\n"));

            helpMsg += "\r\n可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
            return true;
        }

        [EnterCommand(ID = "HelperAI_HelpMe_Command",
            Command = "帮助",
            Description = "获取特定命令或标签的帮助信息",
            SyntaxHint = "[命令名/标签名]",
            SyntaxChecker = "Word",
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

            if (ExtraHelp(MsgDTO))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, "很抱歉，未找到相关帮助信息！");
            return false;
        }

        private bool ExtraHelp(MsgInformationEx MsgDTO)
        {
            var help = ExtraHelps.FirstOrDefault(p => p.Alias.Contains(MsgDTO.Msg));
            if (help == null)
            {
                return false;
            }

            var msg = $"{help.Name}:{help.HelpMsg}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        private bool HelpCommand(MsgInformationEx MsgDTO)
        {
            var commands = CrossWorldAiSvc[MsgDTO.FromGroup].AllAvailableGroupCommands.Where(c => c.Command == MsgDTO.Msg).ToList();
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
                    {"格式", $"{command.Command} {command.SyntaxHint}" },
                    {"描述", command.Description },
                    {"权限", command.AuthorityLevel.ToString() },
                    {"标签", string.Join("-", CrossWorldAiSvc[MsgDTO.FromGroup].CmdTagTree.LocateCmdPath(command).Select(p => p.Tag.ToString())) },
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
            var tag = CrossWorldAiSvc[MsgDTO.FromGroup].CmdTagTree.AllSubTags.FirstOrDefault(p => p.Tag.ToString() == tagName);
            if (tag == null)
            {
                return false;
            }

            var helpMsg = $"【{tagName}】\r\n";
            if (!tag.SubTags.IsNullOrEmpty())
            {
                helpMsg += "当前标签下的子标签有：\r\n";
                var msgList = tag.SubTags.Select(t => $"{t.Tag}{Emoji.AllEmojis().RandElement()}")
                    .Select((msg, i) => i % 2 == 0 ? $"{Emoji.AllEmojis().RandElement()}{msg}" : $"{msg}\r\n").ToList();

                helpMsg += string.Join("", msgList);
                helpMsg += "\r\n";
            }

            if (!tag.SubCmds.IsNullOrEmpty())
            {
                helpMsg += "当前标签下的命令有：\r\n";

                var groups = tag.SubCmds.GroupBy(p => p.ID);
                var subCommands = groups.Select(group => string.Join("/", group.Select(g => g.Command))).Distinct().ToList();
                var msgList = subCommands.Select(cmd => $"【{cmd}】{Emoji.AllEmojis().RandElement()}")
                    .Select((msg, i) => i % 2 == 0 ? $"{Emoji.AllEmojis().RandElement()}{msg}" : $"{msg}\r\n").ToList();

                helpMsg += string.Join("", msgList);
                helpMsg += "\r\n";
            }

            helpMsg += "可以使用 帮助 [标签名] 来查询标签中的具体命令名 或者使用 帮助 [命令名] 来查询具体命令信息。";

            MsgSender.PushMsg(MsgDTO, helpMsg);
            return true;
        }

        [EnterCommand(ID = "HelperAI_ViewSomething",
            Command = "查看",
            Description = "查看某个东西（物品/成就/礼物等等）",
            SyntaxHint = "[名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewSomething(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            // 漂流瓶物品
            if (HonorSvc.FindItem(name) != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<DriftBottleAI>().ViewItem(MsgDTO, param);
            }

            // 漂流瓶成就
            if (HonorSvc.FindHonor(name) != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<DriftBottleAI>().ViewHonor(MsgDTO, param);
            }

            // 礼物
            if (GiftSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<GiftAI>().ViewGift(MsgDTO, param);
            }

            // 宠物技能
            if (PetSkillSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<PetAI>().ViewPetSkill(MsgDTO, param);
            }

            // 宝藏碎片
            if (SegmentSvc.FindSegmentByName(name) != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<SegmentAttachAI>().ViewSegment(MsgDTO, param);
            }

            // 宝藏
            if (SegmentSvc.FindTreasureByName(name) != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<SegmentAttachAI>().ViewTreasure(MsgDTO, param);
            }

            // 菜肴
            if (CookingDietSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<CookingAI>().ViewDiet(MsgDTO, param);
            }

            // 装备
            if (DailyVipShopSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<VipServiceAi>().ViewArmer(MsgDTO, param);
            }

            // 远程地点
            if (ExpeditionSceneSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<ExpeditionAI>().ViewExpedition(MsgDTO, param);
            }

            // 东方自定义卡牌
            if (!string.IsNullOrEmpty(TouhouCardSvc[name]))
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<TouhouCardAi>().ViewCardCard(MsgDTO, param);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关信息！");
            return false;
        }

        [EnterCommand(ID = "HelperAI_LearnSomething",
            Command = "学习",
            Description = "学习某个技能/菜谱/...",
            SyntaxHint = "[名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool LearnSomething(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            // 菜谱
            if (CookingDietSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<CookingAI>().ExchangeMenu(MsgDTO, param);
            }

            // 宠物技能
            if (PetSkillSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<PetAI>().UpgradePetSkill(MsgDTO, param);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关信息！");
            return false;
        }

        [EnterCommand(ID = "HelperAI_ExchangeSomething",
            Command = "兑换",
            Description = "兑换菜谱/礼物/...",
            SyntaxHint = "[名称]",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ExchangeSomething(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            // 菜谱
            if (CookingDietSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<CookingAI>().ExchangeMenu(MsgDTO, param);
            }

            // 礼物
            if (GiftSvc[name] != null)
            {
                return CrossWorldAiSvc[MsgDTO.FromGroup].AIInstance<GiftAI>().MakeGift(MsgDTO, param);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关信息！");
            return false;
        }
    }

    public class ExtraHelpModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string[] Alias { get; set; }

        public string HelpMsg { get; set; }
    }
}
