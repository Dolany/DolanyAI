using Dolany.Ai.Util;

namespace Dolany.Ai.MQ
{
    using System.Text;
    using Resolver;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMQService
    {
        public static RabbitMQService Instance { get; } = new RabbitMQService();

        private readonly ConnectionFactory factory = new ConnectionFactory();

        private readonly IModel channel;

        private readonly object Lock = new object();

        private readonly string routingKey = UtTools.GetConfig("InformationQueueName");

        private RabbitMQService()
        {
            factory.HostName = "localhost"; //RabbitMQ服务在本地运行
            factory.UserName = "guest"; //用户名
            factory.Password = "guest"; //密码

            var connection = this.factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Send(MsgInformation information)
        {
            lock (Lock)
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(information));
                channel.BasicPublish(string.Empty, routingKey, null, body); //开始传递
            }
        }

        public void StartReceive()
        {
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(UtTools.GetConfig("CommandQueueName"), true, consumer);
            consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var command = JsonConvert.DeserializeObject<MsgCommand>(message);

                    Listenser.Instance.ReceivedCommand(command);
                };
        }
    }
}
