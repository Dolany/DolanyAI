using Newbe.Mahua.MahuaEvents;
using System;

namespace Dolany.Ai.MQ.MahuaEvents
{
    using Dolany.Ai.MQ.Resolver;

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
            new Listenser();
        }
    }
}
