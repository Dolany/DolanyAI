﻿using System.Threading.Tasks;

namespace Dolany.Ai.Core.Ai.Game.Jump300Report
{
    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Db;

    [AI(
        Name = nameof(Jump300ReportAI),
        Description = "AI for 300 heros report.",
        IsAvailable = true,
        PriorityLevel = 10)]
    public class Jump300ReportAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "战绩查询",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询300英雄战绩信息",
            Syntax = " [角色名]",
            Tag = "游戏功能",
            SyntaxChecker = "Word",
            IsPrivateAvailabe = true)]
        public void JumpReport(MsgInformationEx MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(MsgDTO, "查询中，请稍候");
            var jr = new JumpReportRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        private static void ReportCallBack(MsgInformationEx MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(MsgDTO, Report);
        }
    }
}