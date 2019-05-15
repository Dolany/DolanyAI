using System;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using Util;

    using Newbe.Mahua;
    using Newbe.Mahua.MahuaEvents;

    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMsgReceive
        : IGroupMessageReceivedMahuaEvent
    {
        public IMahuaApi MahuaApi { get; }

        public GroupMsgReceive(
            IMahuaApi mahuaApi)
        {
            MahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            try
            {
                RabbitMQService.Instance.Send(
                    new MsgInformation
                    {
                        FromGroup = long.Parse(context.FromGroup),
                        FromQQ = long.Parse(context.FromQq),
                        RelationId = string.Empty,
                        Msg = context.Message,
                        Information = InformationType.Message
                    });
            }
            catch (Exception e)
            {
                MahuaModule.RuntimeLogger.Log(e);
            }
        }
    }
}
