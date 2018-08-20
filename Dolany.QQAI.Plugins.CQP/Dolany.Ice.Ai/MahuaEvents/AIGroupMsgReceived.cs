using Newbe.Mahua.MahuaEvents;
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
            try
            {
                AIMgr.Instance.OnGroupMsgReceived(new GroupMsgDTO
                {
                    FromGroup = long.Parse(context.FromGroup),
                    FromQQ = long.Parse(context.FromQq),
                    FromAnonymous = context.FromAnonymous,
                    Msg = context.Message,
                    FullMsg = context.Message,
                });
            }
            catch (Exception ex)
            {
                Utility.SendMsgToDeveloper(ex);
            }
        }
    }
}