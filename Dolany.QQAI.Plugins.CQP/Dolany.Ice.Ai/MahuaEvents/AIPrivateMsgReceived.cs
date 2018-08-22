using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;
using Dolany.Ice.Ai.DolanyAI;

namespace Dolany.Ice.Ai.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class AIPrivateMsgReceived
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public AIPrivateMsgReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            try
            {
                AIMgr.Instance.OnPrivateMsgReceived(new PrivateMsgDTO
                {
                    //SubType = context.,
                    //SendTime = context.SendTime,
                    //FromGroup = long.Parse(context.FromGroup),
                    FromQQ = long.Parse(context.FromQq),
                    //FromAnonymous = context.FromAnonymous,
                    Msg = context.Message,
                    FullMsg = context.Message,
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