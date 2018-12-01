using Newbe.Mahua.MahuaEvents;
using System.Messaging;
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
            SendMessage("send msg test", ".\\private$\\InformationMq");
        }

        private static void SendMessage<T>(T target, string queuePath, MessageQueueTransaction tran = null)
        {
            try
            {
                //连接到本地的队列
                var myQueue = new MessageQueue(queuePath);
                var myMessage = new Message
                {
                    Body = target,
                    Formatter = new XmlMessageFormatter(new[] { typeof(T) })
                };
                //发送消息到队列中
                if (tran == null)
                {
                    myQueue.Send(myMessage);
                }
                else
                {
                    myQueue.Send(myMessage, tran);
                }

                Console.WriteLine("消息已成功发送到" + queuePath + "队列！");
            }
            catch (Exception e)
            {
                MahuaModule.RuntimeLogger.Log(e);
            }
        }
    }
}
