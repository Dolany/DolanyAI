namespace Dolany.Ai.Util
{
    using System.Text;

    using Newtonsoft.Json;

    using RabbitMQ.Client;

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

        public void Send(MsgInformation information)
        {
            information.AiName = UtTools.GetConfig("BindAi");
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(information));
            channel.BasicPublish(string.Empty, UtTools.GetConfig("InformationQueueName"), null, body); //开始传递
        }
    }
}
