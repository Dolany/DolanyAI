﻿namespace Dolany.Ai.MQ.MahuaEvents
{
    using System;

    using Db;
    using Util;

    using Newbe.Mahua;
    using Newbe.Mahua.MahuaEvents;

    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMsgReceive
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMsgReceive(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            RabbitMQService.Instance.Send(
                new MsgInformation
                    {
                        Id = Guid.NewGuid().ToString(),
                        FromGroup = long.Parse(context.FromGroup),
                        FromQQ = long.Parse(context.FromQq),
                        RelationId = string.Empty,
                        Time = DateTime.Now,
                        Msg = context.Message,
                        Information = AiInformation.Message,
                        AiNum = Utility.SelfQQNum
                    });
        }
    }
}
