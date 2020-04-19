using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst
{
    public partial class PetAgainstEngine
    {
        public long GroupNum { get; set; }

        public string BindAi { private get; set; }

        public GamingPet SelfPet { get; set; }

        public GamingPet AimPet { get; set; }

        private GamingPet Winner { get; set; }

        private GamingPet Loser => Winner.QQNum == SelfPet.QQNum ? AimPet : SelfPet;

        private readonly List<string> MsgList = new List<string>();

        private static PetSkillSvc PetSkillSvc => AutofacSvc.Resolve<PetSkillSvc>();

        public void StartGame()
        {
            try
            {
                BeforeGameStart();

                for (var i = 0; i < 12 && !JudgeWinner(); i++)
                {
                    DoSend();
                    SendMessage($"{SelfPet.Name}的回合开始！");
                    BeforeTurnStartTrigger();
                    if (JudgeWinner())
                    {
                        break;
                    }

                    ProcessTurn();
                    if (JudgeWinner())
                    {
                        break;
                    }

                    AfterTurnEndTrigger();
                    if (JudgeWinner())
                    {
                        break;
                    }

                    SwitchPet();
                }

                DoSend();
                ShowResult();
                DoSend();
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                SendMessage("系统异常，对决中止！");
                DoSend();
            }
        }

        #region 扳机

        private void BeforeTurnStartTrigger()
        {
            var beforeStartBuffs = SelfPet.Buffs.Where(p => p.Trigger == CheckTrigger.TurnStart);
            var msg = string.Join("\r\n", beforeStartBuffs.Select(ProcessEffect));
            SendMessage(msg);

            foreach (var buff in SelfPet.Buffs)
            {
                buff.RemainTurn--;
            }

            SelfPet.Buffs.RemoveAll(p => p.RemainTurn <= 0);

            if (AimPet.HP - SelfPet.HP > 25)
            {
                SendMessage($"{SelfPet.Name}发现事情并不简单(滴汗)");
            }
        }

        private string BeAttackedTrigger(GamingPet source, GamingPet dest, int value, DemageType type)
        {
            var buffs = dest.Buffs.Where(b => b.Trigger == CheckTrigger.BeAttacked && ((b.Data["DemageTypes"] as DemageType[])?.Contains(type) ?? false)).ToList();
            foreach (var buff in buffs)
            {
                buff.Data.AddSafe("Source", source);
                buff.Data.AddSafe("Dest", dest);
                buff.Data.AddSafe("Value", value);
                buff.Data.AddSafe("Type", type);
            }
            return string.Join("\r\n", buffs.Select(ProcessEffect));
        }

        private void AfterTurnEndTrigger()
        {
            var buffs = SelfPet.Buffs.Where(b => b.Trigger == CheckTrigger.TurnEnd);
            var msg = string.Join("\r\n", buffs.Select(ProcessEffect));
            SendMessage(msg);

            if (SelfPet.HP - AimPet.HP > 25)
            {
                SendMessage($"{SelfPet.Name}骄傲的摇起了尾巴！");
            }
        }

        private bool DoSkillTrigger()
        {
            var buffs = SelfPet.Buffs.Where(p => p.Trigger == CheckTrigger.DoSkill);
            var msg = string.Join("\r\n", buffs.Select(ProcessEffect)).Trim();
            if (string.IsNullOrEmpty(msg))
            {
                return true;
            }

            SendMessage(msg);
            return false;
        }

        #endregion

        private void ProcessTurn()
        {
            var randSkills = Rander.RandSort(SelfPet.Skills.Keys.Where(s => s != SelfPet.LastSkill).ToArray()).Take(3);
            var skills = SelfPet.Skills.Where((skill, level) => randSkills.Contains(skill.Key)).ToList();

            var selectedIdx = Rander.RandInt(skills.Count);

            var (skillName, skillLevel) = skills.ElementAt(selectedIdx);
            var skillModel = PetSkillSvc[skillName];

            SendMessage($"{SelfPet.Name}施放了 【{skillModel.Name}】(lv.{skillLevel})！");

            SelfPet.LastSkill = skillName;

            if (!DoSkillTrigger())
            {
                return;
            }

            if (skillModel.Data.First().Value.Length == skillLevel)
            {
                SendMessage($"{SelfPet.Name}使出了它的绝技！");
            }

            var msg = ProcessEffect(new GamingEffect()
            {
                Name = skillModel.Name,
                Data = skillModel.Data.ToDictionary(p => p.Key, p => (object)p.Value[skillLevel - 1])
            });

            SendMessage(msg);
        }

        private string ProcessEffect(GamingEffect buff)
        {
            var effect = EffectDic.Keys.First(p => p.Name == buff.Name);
            return EffectDic[effect](buff.Data);
        }

        private void BeforeGameStart()
        {
            var msg = $"{SelfPet.Name}(lv.{SelfPet.Level})   VS   {AimPet.Name}(lv.{AimPet.Level})" +
                      "\r\n对决即将开始，请双方做好准备！";
            SendMessage(msg);

            JudgeFirst();
            var skillName = SkillLevelUpBonus();
            msg = $"{SelfPet.Name}获得了先手\r\n{AimPet.Name}的【{skillName}】获得等级+1";
            SendMessage(msg);
        }

        private void JudgeFirst()
        {
            if (Rander.RandBool())
            {
                return;
            }

            SwitchPet();
        }

        private void SwitchPet()
        {
            var temp = SelfPet;
            SelfPet = AimPet;
            AimPet = temp;
        }

        private string SkillLevelUpBonus()
        {
            var (key, _) = AimPet.Skills.Where(p => p.Value < 5).RandElement();
            AimPet.Skills[key]++;
            return key;
        }

        private void SendMessage(string msg)
        {
            MsgList.Add(msg);
        }

        private void DoSend()
        {
            if (MsgList.IsNullOrEmpty())
            {
                return;
            }

            var msg = string.Join("\r\n", MsgList);
            Thread.Sleep(2000);
            MsgSender.PushMsg(GroupNum, 0 ,msg, BindAi);

            MsgList.Clear();
        }

        private bool JudgeWinner()
        {
            if (SelfPet.HP > 0 && AimPet.HP > 0)
            {
                return false;
            }

            if (SelfPet.HP <= 0 && AimPet.HP <= 0)
            {
                return true;
            }

            Winner = SelfPet.HP > 0 ? SelfPet : AimPet;
            return true;
        }

        private void ShowResult()
        {
            var msg = "对决结束！\r\n";
            if (Winner == null)
            {
                Winner = SelfPet.HP > AimPet.HP ? SelfPet : AimPet;
            }

            msg += $"恭喜{Winner.Name} 获得了胜利！奖励捞瓶子机会一次（当日有效）！";
            var dailyLimit = DailyLimitRecord.Get(Winner.QQNum, "DriftBottleAI_FishingBottle");
            dailyLimit.Decache();
            dailyLimit.Update();

            msg += $"\r\n很遗憾，{Loser.Name}输掉了比赛，在12小时内无法捞瓶子！";
            var buff = new OSPersonBuff
            {
                QQNum = Loser.QQNum,
                Name = "昙天",
                Description = "不可以捞瓶子",
                ExpiryTime = DateTime.Now.AddHours(12),
                IsPositive = false,
                Data = 1,
                Source = Winner.QQNum
            };
            buff.Add();

            SendMessage(msg);
        }
    }
}
