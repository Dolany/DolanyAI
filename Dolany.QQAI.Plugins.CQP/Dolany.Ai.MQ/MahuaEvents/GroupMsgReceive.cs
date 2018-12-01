using Newbe.Mahua.MahuaEvents;
using System;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using Newbe.Mahua;

    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMsgReceive
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMsgReceive(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            // todo 填充处理逻辑
            //throw new NotImplementedException();

            // 不要忘记在MahuaModule中注册
        }
    }
}
