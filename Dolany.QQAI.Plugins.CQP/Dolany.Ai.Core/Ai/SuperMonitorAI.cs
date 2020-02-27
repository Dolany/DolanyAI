using System;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai
{
    public class SuperMonitorAI : AIBase
    {
        public override string AIName { get; set; } = "超级监视器";

        public override string Description { get; set; } = "AI for Monitoring and managing Ais status.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Monitor;

        public BindAiMgr BindAiMgr { get; set; }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            FiltPicMsg(MsgDTO);

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            MsgDTO.IsAlive = AliveStateMgr.GetState(MsgDTO.FromGroup, MsgDTO.FromQQ) == null;

            return !GroupSettingMgr[MsgDTO.FromGroup].IsPowerOn;
        }

        private void FiltPicMsg(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Group)
            {
                var addtionSettings = GroupSettingMgr[MsgDTO.FromGroup].AdditionSettings;
                if (addtionSettings != null && addtionSettings.ContainsKey("禁止图片缓存"))
                {
                    return;
                }
            }

            var guid = Utility.ParsePicGuid(MsgDTO.FullMsg);
            var bindAi = BindAiMgr[MsgDTO.BindAi];
            var cacheInfo = Utility.ReadImageCacheInfo(guid, bindAi.ImagePath);
            if (cacheInfo == null || string.IsNullOrEmpty(cacheInfo.url))
            {
                return;
            }

            PicCacher.Cache(cacheInfo.url, cacheInfo.type);
        }

        [EnterCommand(ID = "MonitorAI_PowerOff",
            Command = "关机 PowerOff",
            Description = "让机器人休眠",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOff(MsgInformationEx MsgDTO, object[] param)
        {
            var groupInfo = GroupSettingMgr[MsgDTO.FromGroup];
            if (!groupInfo.IsPowerOn)
            {
                return false;
            }

            groupInfo.IsPowerOn = false;
            groupInfo.Update();

            MsgSender.PushMsg(MsgDTO, "关机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_PowerOn",
            Command = "开机 PowerOn",
            Description = "唤醒机器人",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public bool PowerOn(MsgInformationEx MsgDTO, object[] param)
        {
            var groupInfo = GroupSettingMgr[MsgDTO.FromGroup];
            if (groupInfo.IsPowerOn)
            {
                return false;
            }

            groupInfo.IsPowerOn = true;
            groupInfo.Update();

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

            var setting = GroupSettingMgr[MsgDTO.FromGroup];
            var expiryDate = $"\r有效期至：{setting.ExpiryTime?.ToLocalTime()}";
            if (setting.ExpiryTime == null || setting.ExpiryTime < DateTime.Now)
            {
                expiryDate += "(已过期)";
            }

            var pState = string.Join("\r", setting.BindAis.Select(p =>
            {
                var msg = $"{p}:";
                if (!BindAiMgr[p].IsConnected)
                {
                    msg += "失联";
                }
                else if (RecentCommandCache.IsTooFreq(p))
                {
                    msg += "过热保护";
                }
                else if(setting.IsPowerOn)
                {
                    msg += "开机";
                }
                else
                {
                    msg += "关机";
                }

                return msg;
            }));
            return $"\r电源状态：\r{pState}{expiryDate}\r";
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
                    var groupList = AIAnalyzer.AnalyzeGroup();
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", groupList.Select(g =>
                    {
                        var groupNum = g.GroupNum == 0 ? "私聊" : Global.AllGroupsDic[g.GroupNum];
                        return $"{groupNum}:{g.CommandCount}";
                    })));
                    return true;
                case "Ai":
                    var aiList = AIAnalyzer.AnalyzeAI();
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", aiList.Select(a => $"{a.AIName}:{a.CommandCount}")));
                    return true;
                case "Time":
                    var timeList = AIAnalyzer.AnalyzeTime();
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", timeList.Select(t => $"{t.Hour}:{t.CommandCount}")));
                    return true;
                case "Command":
                    var commandList = AIAnalyzer.AnalyzeCommand();
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", commandList.Select(c => $"{c.Command}:{c.CommandCount}")));
                    return true;
            }

            return false;
        }
    }
}
