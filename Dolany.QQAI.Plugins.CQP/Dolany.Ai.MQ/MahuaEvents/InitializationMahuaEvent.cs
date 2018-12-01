using Newbe.Mahua.MahuaEvents;
using System;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using System.Messaging;

    using Newbe.Mahua;

    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {
            // todo 填充处理逻辑
            SendMessage("send msg test", "private$\\informationmq");

            // 不要忘记在MahuaModule中注册
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
