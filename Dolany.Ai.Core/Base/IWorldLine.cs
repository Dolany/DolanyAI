﻿using System;
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
    /// <summary>
    /// 时间线基类
    /// </summary>
    public abstract class IWorldLine : IDependency
    {
        public abstract string Name { get; set; }
        public abstract CmdTag CmdTagTree { get; set; }
        public List<AIBase> AIGroup { get; set; }

        public List<EnterCommandAttribute> AllAvailableGroupCommands => AIGroup.SelectMany(ai => ai.AllCmds).ToList();

        public DirtyFilterSvc DirtyFilterSvc { get; set; }
        public RestrictorSvc RestrictorSvc { get; set; }
        public GroupSettingSvc GroupSettingSvc { get; set; }

        public T AIInstance<T>() where T : AIBase
        {
            return AIGroup.FirstOrDefault(ai => ai.GetType().Name == typeof(T).Name) as T;
        }

        public void Init()
        {
            var assembly = GetType().Assembly;
            AIGroup = AutofacSvc.LoadAllInstanceFromClass<AIBase>(assembly);
        }

        /// <summary>
        /// 加载世界线
        /// </summary>
        public void Load()
        {
            Logger.Log($"{Name} WorldLine is starting up...");

            try
            {
                AIGroup = AIGroup.Where(a => a != null && a.Enable).OrderByDescending(a => a.PriorityLevel).ToList();
                var count = AIGroup.Count;

                Logger.Log("AI 加载中...");
                for (var i = 0; i < AIGroup.Count; i++)
                {
                    AIGroup[i].Initialization();

                    Logger.Log($"{AIGroup[i].AIName}({i + 1}/{count})");
                }
                CmdTag.CreateCmdTree(CmdTagTree, AllAvailableGroupCommands);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            // 群聊消息
            if (MsgDTO.FromGroup != 0)
            {
                if (GroupSettingSvc[MsgDTO.FromGroup] == null)
                {
                    return;
                }

                if (Global.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
                {
                    return;
                }
            }

            var msgEx = MsgDTO.ToEx();

            if (!DirtyFilterSvc.Filter(msgEx))
            {
                return;
            }

            MsgCallBack(msgEx);
        }

        /// <summary>
        /// 收款到账事件
        /// </summary>
        /// <param name="model"></param>
        public void OnMoneyReceived(ChargeModel model)
        {
            if (AIGroup.Any(ai => ai.OnMoneyReceived(model)))
            {
            }
        }

        /// <summary>
        /// 群成员变动事件
        /// </summary>
        /// <param name="model"></param>
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
                    var bindAi = RestrictorSvc.AllocateBindAi(MsgDTO);
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
    }
}
