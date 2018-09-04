using Newbe.Mahua.MahuaEvents;
using System;
using Newbe.Mahua;
using Dolany.IWS2000.Ai.DolanyAI;

// ReSharper disable NotAccessedField.Local

namespace Dolany.IWS2000.Ai.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMsgReceived
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMsgReceived(
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
                RuntimeLogger.Log(ex);
            }
        }
    }
}