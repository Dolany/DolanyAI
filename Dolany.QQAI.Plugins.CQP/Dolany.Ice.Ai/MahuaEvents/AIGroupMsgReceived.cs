using Newbe.Mahua.MahuaEvents;
using System;
using Dolany.Ice.Ai.DolanyAI;
using Newbe.Mahua;

namespace Dolany.Ice.Ai.MahuaEvents
{
    /// <inheritdoc />
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class AIGroupMsgReceived
        : IGroupMessageReceivedMahuaEvent
    {
        public IMahuaApi MahuaApi { get; }

        public AIGroupMsgReceived(
            IMahuaApi mahuaApi)
        {
            MahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            try
            {
                AIMgr.Instance.OnMsgReceived(new ReceivedMsgDTO
                {
                    FromGroup = long.Parse(context.FromGroup),
                    FromQQ = long.Parse(context.FromQq),
                    FromAnonymous = context.FromAnonymous,
                    Msg = context.Message,
                    FullMsg = context.Message,
                    MsgType = MsgType.Group
                });
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }
        }
    }
}