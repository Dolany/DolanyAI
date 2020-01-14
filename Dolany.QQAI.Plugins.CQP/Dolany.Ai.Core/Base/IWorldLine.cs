using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Base
{
    public abstract class IWorldLine
    {
        public abstract string Name { get; set; }
        public virtual bool IsDefault { get; } = false;
        protected virtual List<AIBase> AIGroup { get; set; } = new List<AIBase>();
        protected virtual List<IAITool> ToolGroup { get; set; } = new List<IAITool>();

        public string[] ManulOpenAiNames { get; set; }
        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();
        public List<string> OptionalAINames => AIGroup.Where(ai => ai.NeedManualOpeon).Select(ai => ai.AIName).ToList();

        public T AIInstance<T>() where T : AIBase
        {
            return AIGroup.FirstOrDefault(ai => ai.GetType().Name == typeof(T).Name) as T;
        }

        public void Load()
        {
            Logger.Log($"{Name} WorldLine is starting up...");
            var assembly = GetType().Assembly;
            AIGroup = CommonUtil.LoadAllInstanceFromClass<AIBase>(assembly);
            ToolGroup = CommonUtil.LoadAllInstanceFromInterface<IAITool>(assembly);

            try
            {
                AIGroup = AIGroup.Where(a => a != null && a.Enable).OrderByDescending(a => a.PriorityLevel).ToList();
                var count = AIGroup.Count;

                for (var i = 0; i < AIGroup.Count; i++)
                {
                    AIGroup[i].WorldLine = this;
                    AIGroup[i].Initialization();
                    ExtractCommands(AIGroup[i]);

                    Logger.Log($"AI加载进度：{AIGroup[i].AIName}({i + 1}/{count})");
                }

                foreach (var tool in ToolGroup)
                {
                    tool.Work();
                }

                ManulOpenAiNames = AIGroup.Where(ai => ai.NeedManualOpeon).Select(ai => ai.AIName).ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void ExtractCommands(AIBase ai)
        {
            var type = ai.GetType();
            foreach (var method in type.GetMethods())
            {
                foreach (EnterCommandAttribute attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    foreach (var command in attr.CommandsList)
                    {
                        var attrClone = attr.Clone();
                        attrClone.Command = command;
                        AllAvailableGroupCommands.Add(attrClone);
                    }
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            // 群聊消息
            if (MsgDTO.FromGroup != 0)
            {
                if (!Global.AllGroupsDic.ContainsKey(MsgDTO.FromGroup))
                {
                    return;
                }

                if (Global.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
                {
                    return;
                }
            }

            var msgEx = new MsgInformationEx
            {
                Id = MsgDTO.Id,
                Msg = MsgDTO.Msg,
                RelationId = MsgDTO.RelationId,
                Time = MsgDTO.Time,
                FromGroup = MsgDTO.FromGroup,
                FromQQ = MsgDTO.FromQQ,
                BindAi = MsgDTO.BindAi
            };
            if (msgEx.FromQQ < 0)
            {
                msgEx.FromQQ &= 0xFFFFFFFF;
            }

            var msg = msgEx.Msg;
            msgEx.FullMsg = msg;
            msgEx.Command = GenCommand(ref msg);
            msgEx.Msg = msg;
            msgEx.Type = msgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

            MsgCallBack(msgEx);
        }

        public void OnMoneyReceived(ChargeModel model)
        {
            if (AIGroup.Any(ai => ai.OnMoneyReceived(model)))
            {
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void MsgCallBack(MsgInformationEx MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!DirtyFilter.Instance.Filter(MsgDTO))
                    {
                        return;
                    }

                    var availableBindAis = new List<BindAiModel>();
                    if (MsgDTO.Type == MsgType.Group && GroupSettingMgr.Instance[MsgDTO.FromGroup] != null)
                    {
                        availableBindAis = GroupSettingMgr.Instance[MsgDTO.FromGroup].BindAis.Where(p => !RecentCommandCache.IsTooFreq(p))
                            .Select(p => BindAiMgr.Instance[p]).ToList();
                    }
                    else if(!RecentCommandCache.IsTooFreq(MsgDTO.BindAi))
                    {
                        availableBindAis = new List<BindAiModel>(){BindAiMgr.Instance[MsgDTO.BindAi]};
                    }

                    foreach (var ai in AIGroup)
                    {
                        var tempList = availableBindAis;
                        if (ai.IsAdvanced)
                        {
                            tempList = availableBindAis.Where(p => p.IsAdvanced).ToList();
                        }

                        if (tempList.Any())
                        {
                            MsgDTO.BindAi = tempList.RandElement().Name;
                        }
                        else if(ai.PriorityLevel < 25)
                        {
                            break;
                        }

                        if (ai.OnMsgReceived(MsgDTO))
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private static string GenCommand(ref string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return string.Empty;
            }

            var strs = msg.Split(' ');
            if (strs.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length).Trim();
            return command;
        }
    }
}
