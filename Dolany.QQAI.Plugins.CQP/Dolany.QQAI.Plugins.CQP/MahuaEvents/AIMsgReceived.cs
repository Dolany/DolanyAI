using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;

namespace Dolany.QQAI.Plugins.CQP.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class AIMsgReceived
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public AIMsgReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Type = MsgType.Group,
                Aim = long.Parse(context.FromGroup),
                Msg = context.Message
            });
        }
    }
}