﻿using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI;

namespace Dolany.Ice.Ai.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class AIGroupMsgReceived
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public AIGroupMsgReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            RuntimeLogger.Log($"receive group message: fromGroup:{context.FromGroup} fromQQ:{context.FromQq} msg:{context.Message} time:{DateTime.Now}");
            //// 处理群消息。
            //try
            //{
            //    AIMgr.Instance.OnGroupMsgReceived(new GroupMsgDTO()
            //    {
            //        //SubType = context.,
            //        //SendTime = context.SendTime,
            //        FromGroup = long.Parse(context.FromGroup),
            //        FromQQ = long.Parse(context.FromQq),
            //        FromAnonymous = context.FromAnonymous,
            //        Msg = context.Message,
            //        FullMsg = context.Message,
            //        //Font = context.f
            //    });
            //}
            //catch (Exception ex)
            //{
            //    Utility.SendMsgToDeveloper(ex);
            //}
        }
    }
}