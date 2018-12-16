using Newbe.Mahua.MahuaEvents;
using System;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using Dolany.Ai.MQ.Db;
    using Dolany.Ai.Util;

    using Newbe.Mahua;

    /// <summary>
    /// 私聊消息接收事件
    /// </summary>
    public class PrivateMsgReceived
        : IPrivateMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMsgReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessPrivateMessage(PrivateMessageReceivedContext context)
        {
            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(new MsgInformation
                                          {
                                              Id = Guid.NewGuid().ToString(),
                                              FromGroup = 0,
                                              FromQQ = long.Parse(context.FromQq),
                                              RelationId = string.Empty,
                                              Time = DateTime.Now,
                                              Msg = context.Message,
                                              Information = AiInformation.Message,
                                              AiNum = Utility.SelfQQNum
                                          });

                db.SaveChanges();
            }
        }
    }
}
