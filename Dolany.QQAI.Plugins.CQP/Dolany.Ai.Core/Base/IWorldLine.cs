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
    public abstract class IWorldLine : IDependency
    {
        public abstract string Name { get; set; }
        public virtual bool IsDefault { get; } = false;
        public List<AIBase> AIGroup { get; set; }
        protected List<IAITool> ToolGroup { get; set; }

        public string[] ManulOpenAiNames { get; set; }
        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();
        public List<string> OptionalAINames => AIGroup.Where(ai => ai.NeedManualOpeon).Select(ai => ai.AIName).ToList();

        public BindAiSvc BindAiSvc { get; set; }
        public DirtyFilterSvc DirtyFilterSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }
        public RestrictorSvc RestrictorSvc { get; set; }

        public T AIInstance<T>() where T : AIBase
        {
            return AIGroup.FirstOrDefault(ai => ai.GetType().Name == typeof(T).Name) as T;
        }

        public void Init()
        {
            var assembly = GetType().Assembly;
            AIGroup = CommonUtil.LoadAllInstanceFromClass<AIBase>(assembly);
            ToolGroup = CommonUtil.LoadAllInstanceFromInterface<IAITool>(assembly);
        }

        public void Load()
        {
            Logger.Log($"{Name} WorldLine is starting up...");

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

                ToolGroup = ToolGroup.Where(p => p.Enabled).ToList();
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

            var msgEx = ConvertToEx(MsgDTO);

            if (!DirtyFilterSvc.Filter(msgEx))
            {
                return;
            }

            MsgCallBack(msgEx);
        }

        private MsgInformationEx ConvertToEx(MsgInformation MsgDTO)
        {
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

            return msgEx;
        }

        public void OnMoneyReceived(ChargeModel model)
        {
            if (AIGroup.Any(ai => ai.OnMoneyReceived(model)))
            {
            }
        }

        public void OnGroupMemberChanged(GroupMemberChangeModel model)
        {
            if (AIGroup.Any(ai => ai.OnGroupMemberChanged(model)))
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
                    var bindAi = AllocateBindAi(MsgDTO);
                    IEnumerable<AIBase> groups;
                    if (string.IsNullOrEmpty(bindAi))
                    {
                        groups = AIGroup.Where(p => (int) p.PriorityLevel >= (int) AIPriority.System);
                    }
                    else
                    {
                        MsgDTO.BindAi = bindAi;
                        groups = AIGroup;
                    }

                    foreach (var ai in groups)
                    {
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

        private string AllocateBindAi(MsgInformationEx MsgDTO)
        {
            var availableBindAis = new List<BindAiModel>();
            if (MsgDTO.Type == MsgType.Group && GroupSettingSvc[MsgDTO.FromGroup] != null)
            {
                availableBindAis = GroupSettingSvc[MsgDTO.FromGroup].BindAis.Where(p => !RestrictorSvc.IsTooFreq(p))
                    .Select(p => BindAiSvc[p]).ToList();
            }
            else if(!RestrictorSvc.IsTooFreq(MsgDTO.BindAi))
            {
                availableBindAis = new List<BindAiModel>(){BindAiSvc[MsgDTO.BindAi]};
            }

            availableBindAis = availableBindAis.Where(p => p.IsConnected).ToList();
            return availableBindAis.Any() ? availableBindAis.OrderBy(p => RestrictorSvc.GetPressure(p.Name)).First().Name : string.Empty;
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
            msg = msg[command.Length..].Trim();
            return command;
        }
    }
}
