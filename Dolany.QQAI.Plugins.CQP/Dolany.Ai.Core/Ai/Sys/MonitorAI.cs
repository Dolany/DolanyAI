namespace Dolany.Ai.Core.Ai.Sys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Cache;

    using Common;

    using Core;

    using Dolany.Ai.Common;
    using Database;
    using Dolany.Database.Ai;
    using Model;

    using static Common.Utility;

    [AI(
        Name = "监视器",
        Description = "AI for Monitoring and managing Ais status.",
        Enable = true,
        PriorityLevel = 100)]
    public class MonitorAI : AIBase
    {
        private List<long> InactiveGroups = new List<long>();

        public override void Initialization()
        {
            var query = MongoService<ActiveOffGroups>.Get(p => p.AINum == SelfQQNum);
            this.InactiveGroups = query.Select(p => p.GroupNum).ToList();
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            FiltPicMsg(MsgDTO);

            return this.InactiveGroups.Contains(MsgDTO.FromGroup);
        }

        private static void FiltPicMsg(MsgInformationEx MsgDTO)
        {
            var guid = ParsePicGuid(MsgDTO.FullMsg);
            var cacheInfo = ReadImageCacheInfo(guid);
            if (cacheInfo == null || string.IsNullOrEmpty(cacheInfo.url))
            {
                return;
            }

            PicCacher.Cache(cacheInfo.url);
        }

        [EnterCommand(
            Command = "关机 PowerOff",
            Description = "让机器人休眠",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOff(MsgInformationEx MsgDTO, object[] param)
        {
            var selfNum = SelfQQNum;
            var query = MongoService<ActiveOffGroups>.Get(p => p.AINum == selfNum &&
                                                               p.GroupNum == MsgDTO.FromGroup);
            if (!query.IsNullOrEmpty())
            {
                return false;
            }

            MongoService<ActiveOffGroups>.Insert(new ActiveOffGroups
            {
                Id = Guid.NewGuid().ToString(),
                AINum = selfNum,
                GroupNum = MsgDTO.FromGroup,
                UpdateTime = DateTime.Now
            });

            this.InactiveGroups.Add(MsgDTO.FromGroup);
            MsgSender.Instance.PushMsg(MsgDTO, "关机成功！");
            AIMgr.Instance.OnActiveStateChange(false, MsgDTO.FromGroup);
            return true;
        }

        [EnterCommand(
            Command = "开机 PowerOn",
            Description = "唤醒机器人",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOn(MsgInformationEx MsgDTO, object[] param)
        {
            var selfNum = SelfQQNum;
            var query = MongoService<ActiveOffGroups>.Get(p => p.AINum == selfNum &&
                                                               p.GroupNum == MsgDTO.FromGroup);
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            MongoService<ActiveOffGroups>.DeleteMany(query);

            this.InactiveGroups.RemoveAll(p => p == MsgDTO.FromGroup);
            MsgSender.Instance.PushMsg(MsgDTO, "开机成功！");
            AIMgr.Instance.OnActiveStateChange(true, MsgDTO.FromGroup);
            return true;
        }

        [EnterCommand(
            Command = "系统状态 .State",
            Description = "获取机器人当前状态",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool Status(MsgInformationEx MsgDTO, object[] param)
        {
            var startTime = AIAnalyzer.Sys_StartTime;
            var span = DateTime.Now - startTime;
            var timeStr = span.ToString(@"dd\.hh\:mm\:ss");

            var msg = $@"系统已成功运行{timeStr}
共处理{AIAnalyzer.GetCommandCount()}条指令
遇到{AIAnalyzer.GetErrorCount()}个错误{PowerState(MsgDTO)}";

            MsgSender.Instance.PushMsg(MsgDTO, msg);
            return true;
        }

        private string PowerState(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Private)
            {
                return string.Empty;
            }

            return InactiveGroups.Contains(MsgDTO.FromGroup) ? "\r电源状态：关机" : "\r电源状态：开机";
        }

        [EnterCommand(
            Command = "Exception",
            Description = "Get Exception Detail",
            Syntax = "[Index]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool ExceptionMonitor(MsgInformationEx MsgDTO, object[] param)
        {
            var index = (long) param[0];

            var exMsg = AIAnalyzer.GetErrorMsg((int) index);
            if (string.IsNullOrEmpty(exMsg))
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, exMsg);
            return true;
        }

        [EnterCommand(
            Command = "Analyze",
            Description = "Analyze Ais",
            Syntax = "[Aspect]",
            Tag = "系统命令",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public bool Analyze(MsgInformationEx MsgDTO, object[] param)
        {
            var aspect = param[0] as string;

            switch (aspect)
            {
                case "Group":
                    var groupList = AIAnalyzer.AnalyzeGroup();
                    MsgSender.Instance.PushMsg(MsgDTO, string.Join("\r", groupList.Select(g =>
                    {
                        var groupNum = g.GroupNum == 0 ? "私聊" : AIMgr.Instance.AllGroupsDic[g.GroupNum];
                        return $"{groupNum}:{g.CommandCount}";
                    })));
                    return true;
                case "Ai":
                    var aiList = AIAnalyzer.AnalyzeAI();
                    MsgSender.Instance.PushMsg(MsgDTO, string.Join("\r", aiList.Select(a => $"{a.AIName}:{a.CommandCount}")));
                    return true;
                case "Time":
                    var timeList = AIAnalyzer.AnalyzeTime();
                    MsgSender.Instance.PushMsg(MsgDTO, string.Join("\r", timeList.Select(t => $"{t.Hour}:{t.CommandCount}")));
                    return true;
                case "Command":
                    var commandList = AIAnalyzer.AnalyzeCommand();
                    MsgSender.Instance.PushMsg(MsgDTO, string.Join("\r", commandList.Select(c => $"{c.Command}:{c.CommandCount}")));
                    return true;
            }

            return false;
        }
    }
}
