namespace Dolany.Ai.Core.Common
{
    using System.Collections.Generic;
    using System.Linq;

    using Database;

    using Dolany.Ai.Common;

    public static class Global
    {
        public static string AuthCode { get; set; }

        public static bool IsTesting { get; } = bool.Parse(Configger.Instance["IsTesting"]);

        public static IEnumerable<long> TestGroups { get; } = Configger.Instance["TestGroups"].Split(" ").Select(long.Parse);

        public static readonly RabbitMQService CommandInfoService =
            new RabbitMQService(Configger.Instance["InformationQueueName"]);

        public static readonly RabbitMQService CacheInfoService =
            new RabbitMQService(Configger.Instance["CacheResponse"]);
    }

    public enum MsgType
    {
        Private = 1,
        Group = 0
    }
}
