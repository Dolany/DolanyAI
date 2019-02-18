﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Dolany.Ai.Common;
using Dolany.Ai.Core.AITools;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.SyntaxChecker;

namespace Dolany.Ai.Core
{
    /// <summary>
    /// AI管理类
    /// </summary>
    public class AIMgr
    {
        private IList<KeyValuePair<AIBase, AIAttribute>> AIList { get; set; }

        public string[] ManulOpenAiNames { get; set; }

        public static AIMgr Instance { get; } = new AIMgr();

        private List<IAITool> Tools { get; } = new List<IAITool>();

        public List<EnterCommandAttribute> AllAvailableGroupCommands { get; } = new List<EnterCommandAttribute>();

        public List<ISyntaxChecker> Checkers { get; private set; } = new List<ISyntaxChecker>();

        private delegate void MessageCallBack(string msg);

        private event MessageCallBack OnMessageCallBack;

        private AIMgr()
        {
        }

        public void MessagePublish(string message)
        {
            OnMessageCallBack?.Invoke($"{DateTime.Now}: {message}");
        }

        public void Load(Action<string> CallBackFunc = null)
        {
            if (CallBackFunc != null)
            {
                OnMessageCallBack += new MessageCallBack(CallBackFunc);
            }

            try
            {
                Init();

                Logger.Log("start up");
                DbMgr.InitXmls();
                Logger.Log("加载所有可用AI");

                StartAIs();
                Waiter.Instance.Listen();

                Sys_StartTime.Set(DateTime.Now);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// 加载AI
        /// </summary>
        private void StartAIs()
        {
            AIList = AIList.Where(a => a.Value != null && a.Value.Enable).OrderByDescending(a => a.Value.PriorityLevel).ToList();
            var count = AIList.Count;

            for (var i = 0; i < AIList.Count; i++)
            {
                AIList[i].Key.Initialization();
                ExtractCommands(AIList[i].Key);

                Logger.Log($"AI加载进度：{AIList[i].Value.Name}({i + 1}/{count})");
            }

            foreach (var tool in Tools)
            {
                tool.Work();
            }

            ManulOpenAiNames = AIList.Where(ai => ai.Value.NeedManulOpen).Select(ai => ai.Value.Name).ToArray();
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

        private void Init()
        {
            LoadAis();
            LoadTools();
            LoadCheckers();
        }

        private void LoadCheckers()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = assembly.GetTypes()
                .Where(type => typeof(ISyntaxChecker).IsAssignableFrom(type) && type.IsClass)
                .Where(type => type.FullName != null)
                .Select(type => new {type, checker = assembly.CreateInstance(type.FullName) as ISyntaxChecker})
                .Select(@t => @t.checker);

            Checkers = list.ToList();
        }

        private void LoadTools()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass || !typeof(IAITool).IsAssignableFrom(type) || type.IsAbstract)
                {
                    continue;
                }

                if (type.FullName == null)
                {
                    continue;
                }

                var tool = assembly.CreateInstance(type.FullName) as IAITool;
                Tools.Add(tool);
            }

            Logger.Log($"{Tools.Count} tools created.");
            Logger.Log(Global.IsTesting ? "Mode:Testing" : "Mode:Formal");
        }

        private void LoadAis()
        {
            var assembly = Assembly.GetAssembly(typeof(AIBase));
            var list = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(AIBase)))
                .Where(type => type.FullName != null)
                .Select(type => new {type, ai = assembly.CreateInstance(type.FullName) as AIBase})
                .Select(t => new {t, attr = t.type.GetCustomAttribute(typeof(AIAttribute), false) as AIAttribute})
                .Select(t => new KeyValuePair<AIBase, AIAttribute>(t.t.ai, t.attr));

            AIList = list.ToList();
            Logger.Log($"AIList: {(AIList.IsNullOrEmpty() ? 0 : AIList.Count)}");
        }

        [HandleProcessCorruptedStateExceptions]
        public void OnMsgReceived(MsgInformation MsgDTO)
        {
            try
            {
                if (AIList.IsNullOrEmpty())
                {
                    return;
                }

                if (Global.IsTesting && !Global.TestGroups.Contains(MsgDTO.FromGroup))
                {
                    return;
                }

                var msgEx = new MsgInformationEx
                                {
                                    Id = MsgDTO.Id,
                                    Msg = MsgDTO.Msg,
                                    RelationId = MsgDTO.RelationId,
                                    Time = MsgDTO.Time,
                                    FromGroup = MsgDTO.FromGroup,
                                    FromQQ = MsgDTO.FromQQ
                                };
                if (msgEx.FromQQ < 0)
                {
                    msgEx.FromQQ = msgEx.FromQQ & 0xFFFFFFFF;
                }

                var msg = msgEx.Msg;
                msgEx.FullMsg = msg;
                msgEx.Command = GenCommand(ref msg);
                msgEx.Msg = msg;
                msgEx.Type = msgEx.FromGroup == 0 ? MsgType.Private : MsgType.Group;

                MsgCallBack(msgEx);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private void MsgCallBack(MsgInformationEx MsgDTO)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (DirtyFilter.Instance.IsInBlackList(MsgDTO.FromQQ) || !DirtyFilter.Instance.Filter(MsgDTO.FromGroup, MsgDTO.FromQQ, MsgDTO.Msg))
                    {
                        return;
                    }

                    if (!AIList.Any(ai => ai.Key.OnMsgReceived(MsgDTO)))
                    {
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
            if (strs.Length == 0)
            {
                return string.Empty;
            }

            var command = strs[0];
            msg = msg.Substring(command.Length, msg.Length - command.Length).Trim();
            return command;
        }

        public void OnActiveStateChange(bool state, long GroupNum)
        {
            foreach (var ai in AIList)
            {
                ai.Key.OnActiveStateChange(state, GroupNum);
            }
        }
    }
}
