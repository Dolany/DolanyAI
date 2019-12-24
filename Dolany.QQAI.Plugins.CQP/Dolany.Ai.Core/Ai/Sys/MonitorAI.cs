using System;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Sys
{
    public class MonitorAI : AIBase
    {
        public override string AIName { get; set; } = "监视器";

        public override string Description { get; set; } = "AI for Monitoring and managing Ais status.";

        public override int PriorityLevel { get; set; } = 100;

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            FiltPicMsg(MsgDTO);

            return MsgDTO.Type == MsgType.Group && !GroupSettingMgr.Instance[MsgDTO.FromGroup].IsPowerOn;
        }

        private static void FiltPicMsg(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Group)
            {
                var addtionSettings = GroupSettingMgr.Instance[MsgDTO.FromGroup].AdditionSettings;
                if (addtionSettings != null && addtionSettings.ContainsKey("禁止图片缓存"))
                {
                    return;
                }
            }

            var guid = Utility.ParsePicGuid(MsgDTO.FullMsg);
            var bindAi = BindAiMgr.Instance[MsgDTO.BindAi];
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
            var groupInfo = GroupSettingMgr.Instance[MsgDTO.FromGroup];
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
            var groupInfo = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (groupInfo.IsPowerOn)
            {
                return false;
            }

            groupInfo.IsPowerOn = true;
            groupInfo.Update();

            MsgSender.PushMsg(MsgDTO, "开机成功！");
            return true;
        }

        [EnterCommand(ID = "MonitorAI_SelfExcharge",
            Command = "自助充值",
            Description = "使用钻石自助为本群充值指定天数的机器人使用时间(1天=10钻石)",
            Syntax = "[天数]",
            Tag = "系统命令",
            SyntaxChecker = "Long",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public bool SelfExcharge(MsgInformationEx MsgDTO, object[] param)
        {
            var days = (int) (long) param[0];
            if (days <= 0)
            {
                MsgSender.PushMsg(MsgDTO, "天数错误，请重新输入命令！");
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var diamondNeed = days * 10;
            if (osPerson.Diamonds < diamondNeed)
            {
                MsgSender.PushMsg(MsgDTO, "你的钻石余额不足，请添加能天使(2731544408)为好友后，使用【转账】功能转任意金额后将会获得金额*100的钻石，可以【我的状态】命令查看余额！");
                return false;
            }

            AIMgr.Instance.AIInstance<DeveloperOnlyAI>().ChargeTime(MsgDTO, new object[] {MsgDTO.FromGroup, (long)days});
            osPerson.Diamonds -= diamondNeed;
            osPerson.Update();

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

        private static string PowerState(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.Type == MsgType.Private)
            {
                return string.Empty;
            }

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            var expiryDate = $"\r有效期至：{setting.ExpiryTime?.ToLocalTime()}";
            if (setting.ExpiryTime == null || setting.ExpiryTime < DateTime.Now)
            {
                expiryDate += "(已过期)";
            }

            var pState = string.Join("\r", setting.BindAis.Select(p => $"{p}:{(RecentCommandCache.IsTooFreq(p) ? "过热保护" : setting.IsPowerOn ? "开机" : "关机")}"));
            return $"\r电源状态：\r{pState}" + expiryDate;
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
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", groupList.Select(g =>
                    {
                        var groupNum = g.GroupNum == 0 ? "私聊" : Global.AllGroupsDic[g.GroupNum];
                        return $"{groupNum}:{g.CommandCount}";
                    })));
                    return true;
                case "Ai":
                    var aiList = AIAnalyzer.AnalyzeAI(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", aiList.Select(a => $"{a.AIName}:{a.CommandCount}")));
                    return true;
                case "Time":
                    var timeList = AIAnalyzer.AnalyzeTime(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", timeList.Select(t => $"{t.Hour}:{t.CommandCount}")));
                    return true;
                case "Command":
                    var commandList = AIAnalyzer.AnalyzeCommand(MsgDTO.BindAi);
                    MsgSender.PushMsg(MsgDTO, string.Join("\r", commandList.Select(c => $"{c.Command}:{c.CommandCount}")));
                    return true;
            }

            return false;
        }
    }
}
