namespace Dolany.Database
{
    using System;
    using System.Text;

    using Dolany.Ai.Common;
    using Dolany.Database.Ai;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMQService
    {
        public static RabbitMQService Instance { get; } = new RabbitMQService();

        private readonly ConnectionFactory factory = new ConnectionFactory();

        private readonly IModel channel;

        private RabbitMQService()
        {
            factory.HostName = "localhost"; //RabbitMQ服务在本地运行
            factory.UserName = "guest"; //用户名
            factory.Password = "guest"; //密码

            var connection = this.factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Send(MsgCommand command)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
            channel.BasicPublish(string.Empty, CommonUtil.GetConfig("CommandQueueName"), null, body); //开始传递
        }

        public void StartReceive(Action<MsgInformation> CallBack)
        {
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(CommonUtil.GetConfig("InformationQueueName"), true, consumer);
            consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var information = JsonConvert.DeserializeObject<MsgInformation>(message);

                    CallBack(information);
                };
        }
    }
}
