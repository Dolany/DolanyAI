using Dolany.Ai.Common;
using Dolany.Ai.WSMidware.Models;
using Dolany.Database;

namespace Dolany.Ai.WSMidware
{
    public static class Global
    {
        public static readonly ConfigModel Config = Configger<ConfigModel>.Instance.AIConfig;

        public static readonly RabbitMQService MQSvc = new RabbitMQService(Config.MQReceiveQueue);
    }
}
