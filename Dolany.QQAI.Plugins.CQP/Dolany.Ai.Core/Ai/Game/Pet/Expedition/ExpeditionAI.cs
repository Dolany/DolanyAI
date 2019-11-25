using System;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionAI : AIBase
    {
        public override string AIName { get; set; } = "宠物远征";
        public override string Description { get; set; } = "AI for Pet Expedition.";
        public override int PriorityLevel { get; set; } = 10;

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
            var todayExpeditions = ExpeditionSceneMgr.Instance.TodayExpedition();
            var msg = $"请选择远征副本：\r{string.Join("\r\r", todayExpeditions.Select((exp, idx) => $"{idx + 1}:{exp}"))}";
            var selection = Waiter.Instance.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, msg, i => i > 0 && i <= todayExpeditions.Count, MsgDTO.BindAi, 12);
            if (selection < 0)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            var aimExpedition = todayExpeditions[selection - 1];

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var petLevel = PetLevelMgr.Instance[pet.Level];
            var enduranceConsume = PetEnduranceRecord.Get(MsgDTO.FromQQ);

            if (petLevel.Endurance - enduranceConsume.ConsumeTotal < aimExpedition.Endurance)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}的耐力不足({petLevel.Endurance - enduranceConsume.ConsumeTotal}/{aimExpedition.Endurance})！");
                return false;
            }

            var expRec = new ExpeditionRecord
            {
                EndTime = DateTime.Now.AddMinutes(aimExpedition.TimeConsume),
                QQNum = MsgDTO.FromQQ,
                Scene = aimExpedition.Name
            };
            expRec.Insert();

            enduranceConsume.ConsumeTotal += aimExpedition.Endurance;
            enduranceConsume.Update();

            MsgSender.PushMsg(MsgDTO, $"远征开始！目标：{aimExpedition.Name}！");
            return true;
        }

        private void DrawAwards(ExpeditionRecord expeditionRec, MsgInformationEx MsgDTO)
        {
            var expeditionModel = ExpeditionSceneMgr.Instance[expeditionRec.Scene];
            var msg = expeditionModel.Award(MsgDTO.FromQQ);
            MsgSender.PushMsg(MsgDTO, msg);

            expeditionRec.IsDrawn = true;
            expeditionRec.Update();
        }
    }
}
