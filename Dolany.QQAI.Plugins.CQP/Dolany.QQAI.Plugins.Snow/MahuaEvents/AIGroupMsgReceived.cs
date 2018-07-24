using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;

namespace Dolany.QQAI.Plugins.Snow.MahuaEvents
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
            _mahuaApi.SendGroupMessage(context.FromGroup, context.Message);
        }
    }
}