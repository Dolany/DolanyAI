using System;
using System.Text;
using Dolany.Ai.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dolany.Database
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory factory = new ConnectionFactory();

        private readonly string ReceiveQueue;

        private readonly IModel channel;

        public RabbitMQService(string ReceiveQueue)
        {
            factory.HostName = "localhost"; //RabbitMQ服务在本地运行
            factory.UserName = "guest"; //用户名
            factory.Password = "guest"; //密码

            this.ReceiveQueue = ReceiveQueue;

            var connection = this.factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(ReceiveQueue, true, false, false);
        }

        public void Send<T>(T command, string SendQueue)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
            channel.BasicPublish(string.Empty, SendQueue, null, body); //开始传递
        }

        public void StartReceive<T>(Action<T> CallBack)
        {
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(ReceiveQueue, true, consumer);
            consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        var information = JsonConvert.DeserializeObject<T>(message);

                        CallBack(information);
                    }
                    catch (Exception e)
                    {
                        RuntimeLogger.Log(e);
                    }
                };
        }
    }
}
