using Newbe.Mahua.MahuaEvents;
using System;
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
        private readonly IMahuaApi _mahuaApi;

        public PrivateMsgFromGroup(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(PrivateMessageFromGroupReceivedContext context)
        {
            Util.RabbitMQService.Instance.Send(
                new MsgInformation
                {
                    Id = Guid.NewGuid().ToString(),
                    FromGroup = 0,
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
