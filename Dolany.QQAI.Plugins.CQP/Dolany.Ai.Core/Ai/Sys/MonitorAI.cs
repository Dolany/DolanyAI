﻿using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Sys
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core;
    using Base;
    using Cache;
    using Common;
    using Model;
    using Dolany.Database.Ai;

    using static Common.Utility;

    [AI(
        Name = nameof(MonitorAI),
        Description = "AI for Monitor Ais status and emitting heart beat.",
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

        [EnterCommand(
            Command = "功能封印",
            Description = "封印一个群的某个ai功能",
            Syntax = "[群组号] [需要封印的ai名]",
            Tag = "系统命令",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = true)]
        public void SealAi(MsgInformationEx MsgDTO, object[] param)
        {
            var groupNum = (long)param[0];
            var aiName = GetAiRealName(param[1] as string);
            if (string.IsNullOrEmpty(aiName))
            {
                SendMsgToDeveloper("查找ai失败！");
                return;
            }

            var query = MongoService<AISeal>.Get(a => a.GroupNum == groupNum && a.AiName == aiName);
            if (!query.IsNullOrEmpty())
            {
                SendMsgToDeveloper("ai功能已经在封印中！");
                return;
            }

            var aiseal = new AISeal
            {
                Id = Guid.NewGuid().ToString(),
                GroupNum = groupNum,
                AiName = aiName
            };
            MongoService<AISeal>.Insert(aiseal);
            SendMsgToDeveloper("ai封印成功！");
        }

        private static string GetAiRealName(string aiName)
        {
            var list = AIMgr.Instance.AIList;
            foreach (var ai in list)
            {
                var t = ai.GetType();
                var attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
                if (attributes.Length <= 0 || !(attributes[0] is AIAttribute))
                {
                    continue;
                }
                var attr = (AIAttribute)attributes[0];
                if (attr.Name == aiName)
                {
                    return t.Name;
                }
            }

            return string.Empty;
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
遇到{Sys_ErrorCount.Get()}个错误";

            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(
            Command = "临时授权",
            Description = "临时变更某个成员的权限等级，当日有效",
            Syntax = "[@QQ号] 权限名称",
            Tag = "系统命令",
            SyntaxChecker = "At Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailable = false)]
        public void TempAuthorize(MsgInformationEx MsgDTO, object[] param)
        {
            var qqNum = (long)param[0];
            var authName = param[1] as string;

            var validNames = new[] { "开发者", "群主", "管理员", "成员" };
            if (!validNames.Contains(authName))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "权限名称错误！");
                return;
            }

            MongoService<TempAuthorize>.Insert(
                new TempAuthorize
                {
                    Id = Guid.NewGuid().ToString(),
                    AuthDate = DateTime.Now.Date,
                    AuthName = authName,
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = qqNum
                });

            MsgSender.Instance.PushMsg(MsgDTO, "临时授权成功！");
        }

        [EnterCommand(
            Command = "初始化",
            Description = "初始化群成员信息",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = false)]
        public void InitAI(MsgInformationEx MsgDTO, object[] param)
        {
            var today = DateTime.Now.Date;
            var query = MongoService<InitInfo>.Get(p => p.GroupNum == MsgDTO.FromGroup && p.UpdateTime == today).FirstOrDefault();
            if (query != null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "每天只能初始化一次噢~");
                return;
            }

            if (!GroupMemberInfoCacher.RefreshGroupInfo(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "初始化失败，请稍后再试！");
                return;
            }

            query = MongoService<InitInfo>.Get(p => p.GroupNum == MsgDTO.FromGroup).FirstOrDefault();
            if (query == null)
            {
                MongoService<InitInfo>.Insert(new InitInfo { GroupNum = MsgDTO.FromGroup, UpdateTime = DateTime.Now.Date });
            }
            else
            {
                query.UpdateTime = DateTime.Now.Date;
                MongoService<InitInfo>.Update(query);
            }

            MsgSender.Instance.PushMsg(MsgDTO, "初始化成功！");
        }
    }
}
