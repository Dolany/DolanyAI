using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace MQLib
{
    public class MQReceiver
    {
        private Action<string> ReceiveCompleted;

        private bool IsWorking = false;

        public void Receive(string MqPath, Action<string> ReceiveCompleted)
        {
            if (IsWorking)
            {
                return;
            }
            IsWorking = true;
            this.ReceiveCompleted = ReceiveCompleted;
            ReceiveMessage<string>(MqPath);
        }

        private void ReceiveMessage<T>(string queuePath)
        {
            //连接到本地队列
            var myQueue = new MessageQueue(queuePath) { Formatter = new XmlMessageFormatter(new[] { typeof(T) }) };
            try
            {
                //从队列中接收消息
                myQueue.ReceiveCompleted += MyReceiveCompleted;
                myQueue.BeginReceive();
            }
            catch (MessageQueueException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void MyReceiveCompleted(object source, ReceiveCompletedEventArgs asyncResult)
        {
            try
            {
                var myQueue = (MessageQueue)source;
                var message = myQueue.EndReceive(asyncResult.AsyncResult);

                if (ReceiveCompleted == null)
                {
                    return;
                }

                var msgBody = message.Body as string;
                ReceiveCompleted(msgBody);

                myQueue.BeginReceive();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异步接收出错,原因：" + ex.Message);
            }
        }
    }
}
