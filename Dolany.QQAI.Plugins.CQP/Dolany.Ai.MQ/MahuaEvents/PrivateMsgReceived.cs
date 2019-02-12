namespace Dolany.Ai.MQ.MahuaEvents
{
    using Util;

    using Newbe.Mahua;
    using Newbe.Mahua.MahuaEvents;

    /// <summary>
    /// 私聊消息接收事件
    /// </summary>
    public class PrivateMsgReceived
        : IPrivateMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMsgReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessPrivateMessage(PrivateMessageReceivedContext context)
        {
            RabbitMQService.Instance.Send(
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
