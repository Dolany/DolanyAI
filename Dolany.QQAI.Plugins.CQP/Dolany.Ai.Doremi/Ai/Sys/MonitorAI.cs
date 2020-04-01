using System;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.Common;

namespace Dolany.Ai.Doremi.Ai.Sys
{
    [AI(Name = "监视器",
        Description = "AI for Monitoring and managing Ais status.",
        Enable = true,
        PriorityLevel = 100,
        BindAi = "Doremi")]
    public class MonitorAI : AIBase
    {
        public GroupSettingSvc GroupSettingSvc { get; set; }
        public BindAiSvc BindAiSvc { get; set; }
        public PowerStateSvc PowerStateSvc { get; set; }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            FiltPicMsg(MsgDTO);
            return false;
        }

        private void FiltPicMsg(MsgInformationEx MsgDTO)
        {
            var guid = Utility.ParsePicGuid(MsgDTO.FullMsg);
            var bindAi = BindAiSvc[MsgDTO.BindAi];
            var cacheInfo = Utility.ReadImageCacheInfo(guid, bindAi.ImagePath);
            if (cacheInfo == null || string.IsNullOrEmpty(cacheInfo.url))
            {
                return;
            }

            PicCacher.Cache(cacheInfo.url);
        }

        [EnterCommand(ID = "MonitorAI_PowerOff",
            Command = "关机 PowerOff",
            Description = "让所有机器人休眠",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOff(MsgInformationEx MsgDTO, object[] param)
        {
            foreach (var ai in PowerStateSvc.Ais)
            {
                PowerStateSvc.PowerOff(ai);
            }

            MsgSender.PushMsg(MsgDTO, "所有机器人关机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_PowerOffIndex",
            Command = "关机",
            Description = "按编号让指定机器人休眠",
            Syntax = "[编号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOffIndex(MsgInformationEx MsgDTO, object[] param)
        {
            var idx = (int) (long) param[0];
            if (idx < 0 || idx >= PowerStateSvc.Ais.Length)
            {
                MsgSender.PushMsg(MsgDTO, "编号错误！");
                return false;
            }

            var aiName = PowerStateSvc.Ais[idx];
            PowerStateSvc.PowerOff(aiName);
            MsgDTO.BindAi = aiName;

            MsgSender.PushMsg(MsgDTO, "关机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_PowerOn",
            Command = "开机 PowerOn",
            Description = "唤醒所有机器人",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOn(MsgInformationEx MsgDTO, object[] param)
        {
            foreach (var ai in PowerStateSvc.Ais)
            {
                PowerStateSvc.PowerOn(ai);
            }

            MsgSender.PushMsg(MsgDTO, "所有机器人开机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_PowerOnIndex",
            Command = "开机",
            Description = "按编号唤醒指定机器人",
            Syntax = "[编号]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOnIndex(MsgInformationEx MsgDTO, object[] param)
        {
            var idx = (int) (long) param[0];
            if (idx < 0 || idx >= PowerStateSvc.Ais.Length)
            {
                MsgSender.PushMsg(MsgDTO, "编号错误！");
                return false;
            }

            var aiName = PowerStateSvc.Ais[idx];
            PowerStateSvc.PowerOn(aiName);
            MsgDTO.BindAi = aiName;

            MsgSender.PushMsg(MsgDTO, "开机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_Status",
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

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        private string PowerState(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Private)
            {
                return string.Empty;
            }

            var setting = GroupSettingSvc[MsgDTO.FromGroup];
            var expiryDate = $"\r\n有效期至：{setting.ExpiryTime}";

            var pState = string.Join("\r\n",
                PowerStateSvc.Ais.Select((ai, idx) =>
                    $"{idx}号机:{(RecentCommandCache.IsTooFreq(ai) ? "过热保护" : PowerStateSvc.CheckPower(ai) ? "开机" : "关机")}"));
            return $"\r\n电源状态：{pState}" + expiryDate;
        }

        [EnterCommand(ID = "MonitorAI_ExceptionMonitor",
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

            MsgSender.PushMsg(MsgDTO, exMsg);
            return true;
        }

        [EnterCommand(ID = "MonitorAI_Analyze",
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
                    var groupList = AIAnalyzer.AnalyzeGroup(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r\n", groupList.Select(g =>
                    {
                        var groupNum = g.GroupNum == 0 ? "私聊" : Global.AllGroupsDic[g.GroupNum];
                        return $"{groupNum}:{g.CommandCount}";
                    })));
                    return true;
                case "Ai":
                    var aiList = AIAnalyzer.AnalyzeAI(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r\n", aiList.Select(a => $"{a.AIName}:{a.CommandCount}")));
                    return true;
                case "Time":
                    var timeList = AIAnalyzer.AnalyzeTime(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r\n", timeList.Select(t => $"{t.Hour}:{t.CommandCount}")));
                    return true;
                case "Command":
                    var commandList = AIAnalyzer.AnalyzeCommand(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r\n", commandList.Select(c => $"{c.Command}:{c.CommandCount}")));
                    return true;
            }

            return false;
        }
    }
}
