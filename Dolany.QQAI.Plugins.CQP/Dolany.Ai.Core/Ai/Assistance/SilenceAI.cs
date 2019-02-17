using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Assistance
{
    [AI(Name = "禁言",
        Description = "Ai for silence someone",
        Enable = false,
        PriorityLevel = 15)]
    public class SilenceAI : AIBase
    {
        private Dictionary<long, SilenceRule[]> RuleDic = new Dictionary<long, SilenceRule[]>();

        public override void Initialization()
        {
            Load();
        }

        private void Load()
        {
            var query = MongoService<SilenceRule>.Get();
            RuleDic = query.GroupBy(r => r.GroupNum).ToDictionary(r => r.Key, r => r.ToArray());
        }

        [EnterCommand(
            Command = "设置禁言规则",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置固定时长禁言规则（单位：分钟）",
            Syntax = "[内容筛选] [禁言时长]",
            SyntaxChecker = "Word Long",
            Tag = "辅助功能",
            IsPrivateAvailable = false)]
        public bool SetSilence(MsgInformationEx MsgDTO, object[] param)
        {
            var rule = param[0] as string;
            var value = (long) param[1];

            if (value <= 0 )
            {
                MsgSender.Instance.PushMsg(MsgDTO, "参数异常！");
                return false;
            }

            var ruleS = new SilenceRule {GroupNum = MsgDTO.FromGroup, Rule = rule, MinValue = (int) value, MaxValue = (int) value};
            if (RuleDic.Keys.Contains(MsgDTO.FromGroup))
            {
                RuleDic[MsgDTO.FromGroup] = RuleDic[MsgDTO.FromGroup].Append(ruleS).ToArray();
            }
            else
            {
                RuleDic.Add(MsgDTO.FromGroup, new[] {ruleS});
            }

            MongoService<SilenceRule>.Insert(ruleS);
            MsgSender.Instance.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        [EnterCommand(
            Command = "设置禁言规则",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置区间时长禁言规则（单位：分钟）",
            Syntax = "[内容筛选] [禁言时长最小值] [禁言时长最大值]",
            SyntaxChecker = "Word Long Long",
            Tag = "辅助功能",
            IsPrivateAvailable = true)]
        public bool SetSilenceRandom(MsgInformationEx MsgDTO, object[] param)
        {
            var rule = param[0] as string;
            var minValue = (long) param[1];
            var maxValue = (long) param[2];

            if (minValue <= 0 || maxValue <= 0 || minValue > maxValue)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "参数异常！");
                return false;
            }

            var ruleS = new SilenceRule {GroupNum = MsgDTO.FromGroup, Rule = rule, MinValue = (int) minValue, MaxValue = (int) maxValue};
            if (RuleDic.Keys.Contains(MsgDTO.FromGroup))
            {
                RuleDic[MsgDTO.FromGroup] = RuleDic[MsgDTO.FromGroup].Append(ruleS).ToArray();
            }
            else
            {
                RuleDic.Add(MsgDTO.FromGroup, new[] {ruleS});
            }

            MongoService<SilenceRule>.Insert(ruleS);
            MsgSender.Instance.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        [EnterCommand(Command = "禁言规则列表",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "获取禁言规则列表",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "辅助功能",
            IsPrivateAvailable = true)]
        public bool SilenceRuleList(MsgInformationEx MsgDTO, object[] param)
        {
            if (!RuleDic.Keys.Contains(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "当前没有任何禁言规则！");
                return false;
            }

            var rules = RuleDic[MsgDTO.FromGroup];
            var msgList = rules.Select((t, i) => $"{i + 1}. {t.Rule} {t.MinValue}-{t.MinValue}").ToList();

            MsgSender.Instance.PushMsg(MsgDTO, string.Join("\r", msgList));
            return true;
        }

        [EnterCommand(Command = "删除禁言规则",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "按索引删除禁言规则",
            Syntax = "[索引号]",
            SyntaxChecker = "Long",
            Tag = "辅助功能",
            IsPrivateAvailable = true)]
        public bool DeleteSilenceRule(MsgInformationEx MsgDTO, object[] param)
        {
            var idx = (int)((long) param[0] - 1);
            if (!RuleDic.Keys.Contains(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "当前没有任何禁言规则！");
                return false;
            }

            if (idx < 0 || idx >= RuleDic[MsgDTO.FromGroup].Length)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "索引号异常！");
                return false;
            }

            var rule = RuleDic[MsgDTO.FromGroup][idx];
            var rulelist = RuleDic[MsgDTO.FromGroup].ToList();
            rulelist.Remove(rule);
            if (!rulelist.Any())
            {
                RuleDic.Remove(MsgDTO.FromGroup);
            }
            else
            {
                RuleDic[MsgDTO.FromGroup] = rulelist.ToArray();
            }

            MongoService<SilenceRule>.Delete(rule);
            MsgSender.Instance.PushMsg(MsgDTO, "删除成功！");
            return true;
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (!RuleDic.Keys.Contains(MsgDTO.FromGroup))
            {
                return false;
            }

            var rules = RuleDic[MsgDTO.FromGroup];
            var rule = rules.FirstOrDefault(r => MsgDTO.FullMsg.Contains(r.Rule));
            if (rule == null)
            {
                return false;
            }

            SendSilence(MsgDTO, rule);
            return true;
        }

        private void SendSilence(MsgInformationEx MsgDTO, SilenceRule rule)
        {
            var duringTime = rule.MinValue + CommonUtil.RandInt(rule.MaxValue - rule.MinValue);
            Waiter.Instance.WaitForRelationId(MsgDTO, duringTime.ToString());

            var msg = $"当前禁言规则：\r禁言内容：{rule.Rule}\r禁言时长(分钟)：{rule.MinValue}-{rule.MaxValue}";
            MsgSender.Instance.PushMsg(MsgDTO, msg, true);
        }
    }
}
