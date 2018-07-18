﻿using Newbe.Mahua.MahuaEvents;
using Newbe.Mahua;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.CQP.MahuaEvents
{
    /// <summary>
    /// 来自好友的私聊消息接收事件
    /// </summary>
    public class PrivateMessageFromFriendReceivedMahuaEvent1
        : IPrivateMessageFromFriendReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageFromFriendReceivedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessFriendMessage(PrivateMessageFromFriendReceivedContext context)
        {
            // 戳一戳
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Shake()
                .Done();

            // 嘤嘤嘤，换行，重复消息
            _mahuaApi.SendPrivateMessage(context.FromQq)
                .Text("嘤嘤嘤：")
                .Newline()
                .Text(context.Message)
                .Done();

            // 异步发送消息，不能使用 _mahuaApi 实例，需要另外开启Session
            Task.Factory.StartNew(() =>
            {
                using (var robotSession = MahuaRobotManager.Instance.CreateSession())
                {
                    var api = robotSession.MahuaApi;
                    api.SendPrivateMessage(context.FromQq, "异步的嘤嘤嘤");
                }
            });
        }
    }
}