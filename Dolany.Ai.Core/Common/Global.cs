using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Common
{
    public class Global
    {
        public static bool IsTesting { get; } = DefaultConfig.IsTesting;

        public static long DeveloperNumber { get; } = DefaultConfig.DeveloperNumber;

        public static long SysMsgNumber { get; } = 10000;

        public static long AnonymousNumber { get; } = 80000000;

        public static IEnumerable<long> TestGroups { get; } = DefaultConfig.TestGroups;

        public static AIConfigBase DefaultConfig => Configger<AIConfigBase>.Instance.AIConfig;

        public static readonly RabbitMQService CommandInfoService = new RabbitMQService(DefaultConfig.InformationQueueName);

        public static Action<string> MsgPublish;
    }
}
