using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI;

namespace Dolany.Ice.Ai.MahuaEvents
{
    /// <inheritdoc />
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class AIPrivateMsgReceived
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        public IMahuaApi MahuaApi { get; }

        public AIPrivateMsgReceived(
            IMahuaApi mahuaApi)
        {
            MahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            try
            {
                AIMgr.Instance.OnMsgReceived(new ReceivedMsgDTO
                {
                    //SubType = context.,
                    //SendTime = context.SendTime,
                    //FromGroup = long.Parse(context.FromGroup),
                    FromQQ = long.Parse(context.FromQq),
                    //FromAnonymous = context.FromAnonymous,
                    Msg = context.Message,
                    FullMsg = context.Message,
                    MsgType = MsgType.Private
                    //Font = context.f
                });
            }
            catch (Exception ex)
            {
                Utility.SendMsgToDeveloper(ex);
            }
        }
    }
}