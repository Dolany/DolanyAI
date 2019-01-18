using Dolany.Database.Sqlite;

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
    using Database.Sqlite.Model;

    using Model;

    using static Common.Utility;

    [AI(
        Name = nameof(MonitorAI),
        Description = "AI for Monitoring and managing Ais status.",
        IsAvailable = true,
        PriorityLevel = 100)]
    public class MonitorAI : AIBase
    {
        private List<long> InactiveGroups = new List<long>();

        public override void Work()
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

            if (!this.InactiveGroups.Contains(MsgDTO.FromGroup))
            {
                return false;
            }

            Sys_CommandCount.Minus();
            return true;
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
        public void PowerOff(MsgInformationEx MsgDTO, object[] param)
        {
            var selfNum = SelfQQNum;
            var query = MongoService<ActiveOffGroups>.Get(p => p.AINum == selfNum &&
                                                               p.GroupNum == MsgDTO.FromGroup);
            if (!query.IsNullOrEmpty())
            {
                return;
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
        }

        [EnterCommand(
            Command = "开机 PowerOn",
            Description = "唤醒机器人",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailable = false)]
        public void PowerOn(MsgInformationEx MsgDTO, object[] param)
        {
            var selfNum = SelfQQNum;
            var query = MongoService<ActiveOffGroups>.Get(p => p.AINum == selfNum &&
                                                               p.GroupNum == MsgDTO.FromGroup);
            if (query.IsNullOrEmpty())
            {
                return;
            }

            MongoService<ActiveOffGroups>.DeleteMany(query);

            this.InactiveGroups.RemoveAll(p => p == MsgDTO.FromGroup);
            MsgSender.Instance.PushMsg(MsgDTO, "开机成功！");
            AIMgr.Instance.OnActiveStateChange(true, MsgDTO.FromGroup);
        }

        [EnterCommand(
            Command = "系统状态 .State",
            Description = "获取机器人当前状态",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public void Status(MsgInformationEx MsgDTO, object[] param)
        {
            var startTime = Sys_StartTime.Get();
            var span = DateTime.Now - startTime;
            var timeStr = span.ToString(@"dd\.hh\:mm\:ss");

            var msg = $@"系统已成功运行{timeStr}
共处理{Sys_CommandCount.Get()}条指令
遇到{Sys_ErrorCount.GetCount()}个错误{PowerState(MsgDTO)}";

            MsgSender.Instance.PushMsg(MsgDTO, msg);
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
            Command = "初始化",
            Description = "初始化群成员信息",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public void InitAi(MsgInformationEx MsgDTO, object[] param)
        {
            var key = $"InitInfo-{MsgDTO.FromGroup}";
            var response = SqliteCacheService.Get<InitInfoCache>(key);

            if (response != null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "每天只能初始化一次噢~");
                return;
            }

            if (!GroupMemberInfoCacher.RefreshGroupInfo(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "初始化失败，请稍后再试！");
                return;
            }

            var model = new InitInfoCache {GroupNum = MsgDTO.FromGroup};
            SqliteCacheService.Cache(key, model, CommonUtil.UntilTommorow());

            MsgSender.Instance.PushMsg(MsgDTO, "初始化成功！");
        }

        [EnterCommand(
            Command = "Exception",
            Description = "Get Exception Detail",
            Syntax = "[Index]",
            Tag = "系统命令",
            SyntaxChecker = "Long", 
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public void ExceptionMonitor(MsgInformationEx MsgDTO, object[] param)
        {
            var index = (long) param[0];

            var exMsg = Sys_ErrorCount.GetMsg((int) index);
            if (string.IsNullOrEmpty(exMsg))
            {
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, exMsg);
        }
    }
}
