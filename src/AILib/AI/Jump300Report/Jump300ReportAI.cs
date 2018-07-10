﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AILib.AI.Jump300Report;
using System.ComponentModel.Composition;

namespace AILib
{
    [AI(
        Name = "Jump300ReportAI",
        Description = "AI for 300 heros report.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class Jump300ReportAI : AIBase
    {
        public Jump300ReportAI()
            : base()
        {
        }

        public override void Work()
        {
        }

        [GroupEnterCommandAttribute(
            Command = "战绩查询",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查询300英雄战绩信息",
            Syntax = " [角色名]",
            Tag = "战绩查询功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void JumpReport(GroupMsgDTO MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "查询中，请稍候"
            });
            JumpReportRequestor jr = new JumpReportRequestor(MsgDTO, ReportCallBack);
            Task.Run(() => jr.Work());
        }

        public void ReportCallBack(GroupMsgDTO MsgDTO, string Report)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO()
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = Report
            });
        }
    }
}