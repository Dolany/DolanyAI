using Newbe.Mahua.MahuaEvents;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using Newbe.Mahua;
    using Dolany.Ai.MQ.Db;
    using System;

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
            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(new MsgInformation
                {
                    Id = Guid.NewGuid().ToString(),
                    FromGroup = long.Parse(context.FromGroup),
                    FromQQ = long.Parse(context.FromQq),
                    RelationId = string.Empty,
                    Time = DateTime.Now,
                    Msg = context.Message
                });

                db.SaveChanges();
            }
        }
    }
}
