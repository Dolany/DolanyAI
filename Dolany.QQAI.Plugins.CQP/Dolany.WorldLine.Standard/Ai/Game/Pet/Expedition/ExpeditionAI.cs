using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition
{
    public class ExpeditionAI : AIBase
    {
        public override string AIName { get; set; } = "宠物远征";
        public override string Description { get; set; } = "AI for Pet Expedition.";
        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        private static ExpeditionSceneMgr ExpeditionSceneMgr => AutofacSvc.Resolve<ExpeditionSceneMgr>();
        private static PetLevelMgr PetLevelMgr => AutofacSvc.Resolve<PetLevelMgr>();
        private static HonorHelper HonorHelper => AutofacSvc.Resolve<HonorHelper>();

        [EnterCommand(ID = "ExpeditionAI_Expedite",
            Command = "宠物远征",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "开始宠物远征，或者领取远征奖励",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool Expedite(MsgInformationEx MsgDTO, object[] param)
        {
            var expeditionRec = ExpeditionRecord.GetLastest(MsgDTO.FromQQ);
            if (expeditionRec == null)
            {
                return StartExpedite(MsgDTO);
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            if (expeditionRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}还在远征中，请于 {expeditionRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            if (expeditionRec.IsDrawn)
            {
                return StartExpedite(MsgDTO);
            }

            DrawAwards(expeditionRec, MsgDTO);
            return true;
        }

        private bool StartExpedite(MsgInformationEx MsgDTO)
        {
            var extEndur = VipArmerRecord.Get(MsgDTO.FromQQ).CheckArmer("耐力护符") ? 10 : 0;

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var petLevel = PetLevelMgr[pet.Level];
            var enduranceConsume = PetEnduranceRecord.Get(MsgDTO.FromQQ);
            var curEndurance = petLevel.Endurance - enduranceConsume.ConsumeTotal + extEndur;

            var todayExpeditions = ExpeditionSceneMgr.TodayExpedition().Where(p => p.Endurance <= curEndurance).ToList();
            if (todayExpeditions.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}已经累的完全动不了了！");
                return false;
            }

            var msg = $"请选择远征副本：\r{string.Join("\r", todayExpeditions.Select((exp, idx) => $"{idx + 1}:{exp.ToString(curEndurance)}"))}";
            var selection = WaiterSvc.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, msg, i => i > 0 && i <= todayExpeditions.Count, MsgDTO.BindAi, 12, false);
            if (selection < 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            var aimExpedition = todayExpeditions[selection - 1];
            var expRec = new ExpeditionRecord
            {
                EndTime = DateTime.Now.AddMinutes(aimExpedition.TimeConsume),
                QQNum = MsgDTO.FromQQ,
                Scene = aimExpedition.Name
            };
            expRec.Insert();

            enduranceConsume.ConsumeTotal += aimExpedition.Endurance;
            enduranceConsume.Update();

            MsgSender.PushMsg(MsgDTO, $"远征开始！目标：【{aimExpedition.Name}】！(请于{expRec.EndTime:yyyy-MM-dd HH:mm:ss}后使用 宠物远征 命令回收远征奖励！)");
            return true;
        }

        private static void DrawAwards(ExpeditionRecord expeditionRec, MsgInformationEx MsgDTO)
        {
            var expeditionModel = ExpeditionSceneMgr[expeditionRec.Scene];
            var award = expeditionModel.Award(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, award.ToString());

            expeditionRec.IsDrawn = true;
            expeditionRec.Update();

            var history = ExpeditionHistory.Get(MsgDTO.FromQQ);
            history.AddScene(expeditionModel.Name);
            history.EnduranceConsume += expeditionModel.Endurance;
            history.FlavoringTotal += award.Flavorings.Count;
            history.GoldsTotal += award.Gold;
            history.ItemBonusCount += award.Items.Count;
            history.ItemBonusPriceTotal += award.Items.Sum(item => HonorHelper.FindItem(item).Price);

            history.Update();
        }

        [EnterCommand(ID = "ExpeditionAI_ViewExpedition",
            Command = "查看远征地点",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看某个远征地点的信息",
            Syntax = "[远征地点名称]",
            Tag = "宠物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool ViewExpedition(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var scene = ExpeditionSceneMgr[name];
            if (scene == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关地点！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, scene.ToString());
            return true;
        }

        [EnterCommand(ID = "ExpeditionAI_MyExpeditionHistory",
            Command = "我的远征记录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看我的远征记录",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool MyExpeditionHistory(MsgInformationEx MsgDTO, object[] param)
        {
            var history = ExpeditionHistory.Get(MsgDTO.FromQQ);
            if (history.SceneDic.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "尚未记录到远征信息！");
                return false;
            }

            var hotScenes = history.SceneDic.OrderByDescending(p => p.Value).Take(3).ToList();
            var msg = $"你最热衷的远征地点：{string.Join(",", hotScenes.Select(s => $"{s.Key}*{s.Value}次"))}\r";
            msg += $"共获得金币：{history.GoldsTotal.CurencyFormat()}\r";
            msg += $"共获得调味料：{history.FlavoringTotal}个\r";
            msg += $"共获得物品：{history.ItemBonusCount}个\r";
            msg += $"物品总价值：{history.ItemBonusPriceTotal.CurencyFormat()}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "ExpeditionAI_ResolveItem",
            Command = "分解 分解物品",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "将某个物品分解为调味料",
            Syntax = "[物品名]",
            Tag = "宠物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true,
            DailyLimit = 5,
            TestingDailyLimit = 5)]
        public bool ResolveItem(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var item = HonorHelper.FindItem(name);
            if (item == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关物品！");
                return false;
            }

            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!itemColle.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未持有该物品！");
                return false;
            }

            itemColle.ItemConsume(name);
            itemColle.Update();

            var count = Math.Max(item.Price / 20, 1);
            var flavorings = ExpeditionSceneMgr.RandFlavorings(count);

            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            cookingRec.FlavoringIncome(flavorings);
            cookingRec.Update();

            var msg = string.Join(",", flavorings.Select(p => $"{p.Key}×{p.Value}"));
            MsgSender.PushMsg(MsgDTO, $"分解成功！你获得了：\r{msg}");
            return true;
        }
    }
}
