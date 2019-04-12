﻿using Newbe.Mahua.MahuaEvents;
using Dolany.Ai.Util;
using Newbe.Mahua;

namespace Dolany.Ai.MQ.MahuaEvents
{
    /// <summary>
    /// 来自群成员的私聊消息接收事件
    /// </summary>
    public class PrivateMsgFromGroup
        : IPrivateMessageFromGroupReceivedMahuaEvent
    {
        public IMahuaApi MahuaApi { get; }

        public PrivateMsgFromGroup(
            IMahuaApi mahuaApi)
        {
            MahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(PrivateMessageFromGroupReceivedContext context)
        {
            Util.RabbitMQService.Instance.Send(
                new MsgInformation
                {
                    FromGroup = 0,
                    FromQQ = long.Parse(context.FromQq),
                    RelationId = string.Empty,
                    Msg = context.Message,
                    Information = AiInformation.Message
                });
        }
    }
}
