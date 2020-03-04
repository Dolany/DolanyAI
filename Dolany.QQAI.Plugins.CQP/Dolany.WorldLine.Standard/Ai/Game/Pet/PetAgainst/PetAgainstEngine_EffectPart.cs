using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst
{
    public partial class PetAgainstEngine
    {
        private delegate string EffectDel(Dictionary<string, object> data);

        private readonly Dictionary<PetGamingEffectAttribute, EffectDel> EffectDic = new Dictionary<PetGamingEffectAttribute, EffectDel>();

        public PetAgainstEngine()
        {
            var type = GetType();
            var methods = type.GetMethods().Where(m => m.CustomAttributes.Any(attr => attr.AttributeType == typeof(PetGamingEffectAttribute)));
            foreach (var method in methods)
            {
                EffectDic.AddSafe(method.GetCustomAttribute(typeof(PetGamingEffectAttribute)) as PetGamingEffectAttribute, method.CreateDelegate(typeof(EffectDel), this) as EffectDel);
            }
        }

        #region 伤害结算

        private string DoDemage(GamingPet source, GamingPet dest, DemageType type, int value, string title)
        {
            var realDemage = GetRealDemageValue(source, dest, type, value);
            if (realDemage <= 0)
            {
                return null;
            }

            DoRealDemage(dest, realDemage);
            var msg = $"{source.Name}对{dest.Name}造成了{realDemage}点{type.ToString()}伤害({title}),{dest.Name}剩余{dest.HP}点生命值！";
            var triggerMsg = BeAttackedTrigger(source, dest, realDemage, type);
            if (!string.IsNullOrWhiteSpace(triggerMsg))
            {
                msg += $"\r{triggerMsg}";
            }

            return msg;
        }

        private static void DoRealDemage(GamingPet dest, int realValue)
        {
            dest.HP -= realValue;
            dest.HP = Math.Max(dest.HP, 0);
        }

        private int GetRealDemageValue(GamingPet source, GamingPet dest, DemageType type, int value)
        {
            var demage = CalSourceDemage(source, type, value);
            return CalDestDemage(source, dest, type, demage);
        }

        private int CalSourceDemage(GamingPet source, DemageType type, int value)
        {
            var result = value;
            var buffs = type switch
            {
                DemageType.物理 => source.Buffs.Where(p => p.Trigger == CheckTrigger.PhyAttackFix),
                DemageType.魔法 => source.Buffs.Where(p => p.Trigger == CheckTrigger.MagicAttackFix),
                DemageType.毒系 => source.Buffs.Where(p => p.Trigger == CheckTrigger.PoisionAttackFix),
                //DemageType.真实 => 
                _ => new List<GamingBuff>()
            };
            foreach (var buff in buffs)
            {
                buff.Data.AddSafe("Source", source);
                buff.Data.AddSafe("Value", result);
                result = int.Parse(ProcessEffect(buff));
            }

            return result;
        }

        private int CalDestDemage(GamingPet source, GamingPet dest, DemageType type, int value)
        {
            var result = value;
            var buffs = type switch
            {
                DemageType.物理 => dest.Buffs.Where(p => p.Trigger == CheckTrigger.PhyDefenceFix),
                DemageType.魔法 => dest.Buffs.Where(p => p.Trigger == CheckTrigger.MagicDefenceFix),
                DemageType.毒系 => dest.Buffs.Where(p => p.Trigger == CheckTrigger.PoisionDefenceFix),
                //DemageType.真实 => 
                _ => new List<GamingBuff>()
            };
            foreach (var buff in buffs)
            {
                buff.Data.AddSafe("Source", source);
                buff.Data.AddSafe("Dest", dest);
                buff.Data.AddSafe("Value", result);
                result = int.Parse(ProcessEffect(buff));
            }

            return result;
        }

        #endregion

        #region Buff

        [PetGamingEffect(Name = "中毒")]
        public string 中毒Buff(Dictionary<string, object> data)
        {
            return DoDemage(AimPet, SelfPet, DemageType.毒系, (int)data["Hurt"], "中毒");
        }

        [PetGamingEffect(Name = "百分比防御")]
        public string 百分比防御Buff(Dictionary<string, object> data)
        {
            var defence = (int) data["Defence"];
            var value = (int) data["Value"];

            return defence >= 100 ? "0" : (value * (100 - defence) / 100).ToString();
        }

        [PetGamingEffect(Name = "百分比伤害")]
        public string 百分比伤害Buff(Dictionary<string, object> data)
        {
            var value = (int) data["Value"];
            var rate = (int) data["Rate"];

            return (value * (rate + 100) / 100).ToString();
        }

        [PetGamingEffect(Name = "物理攻击增加")]
        public string 物理攻击增加Buff(Dictionary<string, object> data)
        {
            var value = (int) data["Value"];
            var addPhy = (int) data["addPhy"];

            return (value + addPhy).ToString();
        }

        [PetGamingEffect(Name = "反击Buff")]
        public string 反击Buff(Dictionary<string, object> data)
        {
            var hurt = (int) data["Hurt"];
            var source = data["Source"] as GamingPet;
            var dest = data["Dest"] as GamingPet;

            var msg = DoDemage(dest, source, DemageType.魔法, hurt, "反击");
            SendMessage(msg);

            var value = (int) data["Value"];
            return value.ToString();
        }

        [PetGamingEffect(Name = "闷棍")]
        public string 闷棍Buff(Dictionary<string, object> data)
        {
            var rate = (int) data["Rate"];
            return Rander.RandInt(100) < rate ? "技能释放失败！" : string.Empty;
        }

        #endregion

        #region 技能

        [PetGamingEffect(Name = "撕咬")]
        public string 撕咬(Dictionary<string, object> data)
        {
            return DoDemage(SelfPet, AimPet, DemageType.物理, (int) data["Hurt"], "撕咬");
        }

        [PetGamingEffect(Name = "爪击")]
        public string 爪击(Dictionary<string, object> data)
        {
            var msg = DoDemage(SelfPet, AimPet, DemageType.物理, (int) data["Hurt"], "爪击");
            if (AimPet.Buffs.All(b => b.Trigger != CheckTrigger.PhyDefenceFix))
            {
                return msg;
            }

            AimPet.Buffs.RemoveAll(p => p.Trigger == CheckTrigger.PhyDefenceFix);
            msg += $"\r已移除{AimPet.Name}的所有物理防御！";
            return msg;
        }

        [PetGamingEffect(Name = "魔法飞弹")]
        public string 魔法飞弹(Dictionary<string, object> data)
        {
            return DoDemage(SelfPet, AimPet, DemageType.魔法, (int) data["Hurt"], "魔法飞弹");
        }

        [PetGamingEffect(Name = "魔法反制")]
        public string 魔法反制(Dictionary<string, object> data)
        {
            SelfPet.Buffs.Add(new GamingBuff()
            {
                Name = "百分比防御",
                Data = new Dictionary<string, object>()
                {
                    {"Defence", data["Defence"] }
                },
                RemainTurn = (int)data["Turn"],
                Trigger = CheckTrigger.MagicDefenceFix
            });

            return $"魔法防御增加{data["Defence"]}%，持续{data["Turn"]}个回合(魔法反制)";
        }

        [PetGamingEffect(Name = "反击")]
        public string 反击(Dictionary<string, object> data)
        {
            SelfPet.Buffs.Add(new GamingBuff()
            {
                Name = "反击Buff",
                Data = new Dictionary<string, object>()
                {
                    {"Hurt", data["Hurt"] }
                },
                RemainTurn = (int)data["Turn"],
                Trigger = CheckTrigger.PhyDefenceFix
            });

            return $"受到物理伤害时，反弹{data["Hurt"]}点魔法伤害，持续{data["Turn"]}个回合";
        }

        [PetGamingEffect(Name = "闷棍")]
        public string 闷棍(Dictionary<string, object> data)
        {
            var msg = DoDemage(SelfPet, AimPet, DemageType.物理, (int)data["Hurt"], "闷棍");
            AimPet.Buffs.Add(new GamingBuff()
            {
                Name = "闷棍",
                Data = new Dictionary<string, object>()
                {
                    {"Rate", data["Rate"] }
                },
                RemainTurn = (int)data["Turn"] + 1,
                Trigger = CheckTrigger.DoSkill
            });

            return $"{msg}\r对手技能失败率增加{data["Rate"]}%，持续{data["Turn"]}回合";
        }

        [PetGamingEffect(Name = "盾墙")]
        public string 盾墙(Dictionary<string, object> data)
        {
            SelfPet.Buffs.Add(new GamingBuff()
            {
                Name = "百分比防御",
                Data = new Dictionary<string, object>()
                {
                    {"Defence", data["Defence"] }
                },
                RemainTurn = (int)data["Turn"],
                Trigger = CheckTrigger.PhyDefenceFix
            });

            return $"物理防御增加{data["Defence"]}%，持续{data["Turn"]}个回合(盾墙)";
        }

        [PetGamingEffect(Name = "法术共鸣")]
        public string 法术共鸣(Dictionary<string, object> data)
        {
            SelfPet.Buffs.Add(new GamingBuff()
            {
                Name = "百分比伤害",
                Data = new Dictionary<string, object>()
                {
                    {"Rate", data["Rate"] }
                },
                RemainTurn = (int)data["Turn"] + 1,
                Trigger = CheckTrigger.MagicAttackFix
            });

            return $"魔法伤害增加{data["Rate"]}%，持续{data["Turn"]}个回合(法术共鸣)";
        }

        [PetGamingEffect(Name = "烈焰喷泉")]
        public string 烈焰喷泉(Dictionary<string, object> data)
        {
            var msg = DoDemage(SelfPet, AimPet, DemageType.魔法, (int) data["Magic"], "烈焰喷泉");
            if (AimPet.Buffs.Any(buff => buff.Trigger == CheckTrigger.MagicDefenceFix))
            {
                msg += $"\r{DoDemage(SelfPet, AimPet, DemageType.物理, (int) data["Phy"], "烈焰喷泉")}";
            }

            return msg;
        }

        [PetGamingEffect(Name = "涂毒")]
        public string 涂毒(Dictionary<string, object> data)
        {
            AimPet.Buffs.Add(new GamingBuff()
            {
                Name = "中毒",
                Data = new Dictionary<string, object>()
                {
                    {"Hurt", (int)data["Hurt"] }
                },
                RemainTurn = (int)data["Turn"] + 1
            });

            var msg = $"对方已进入中毒状态！每回合受到{(int) data["Hurt"]}点毒系伤害，持续{(int) data["Turn"]}回合";
            return msg;
        }

        [PetGamingEffect(Name = "圣光洗礼")]
        public string 圣光洗礼(Dictionary<string, object> data)
        {
            SelfPet.Buffs.Clear();
            SelfPet.Buffs.Add(new GamingBuff()
            {
                Name = "物理攻击增加",
                Data = new Dictionary<string, object>()
                {
                    {"addPhy", (int)data["Phy"] }
                },
                RemainTurn = (int)data["Turn"],
                Trigger = CheckTrigger.PhyAttackFix
            });

            var msg = $"自身所有buff已清除！物理攻击增加{(int) data["Phy"]}，持续{(int) data["Turn"]}回合";
            return msg;
        }

        [PetGamingEffect(Name = "穿刺打击")]
        public string 穿刺打击(Dictionary<string, object> data)
        {
            return DoDemage(SelfPet, AimPet, DemageType.真实, (int) data["Hurt"], "穿刺打击");
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PetGamingEffectAttribute : Attribute
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }
    }

    public enum DemageType
    {
        物理,
        魔法,
        毒系,
        真实
    }
}
