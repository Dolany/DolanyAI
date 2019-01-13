namespace Dolany.Ai.MQ.MahuaEvents
{
    using System;

    using Db;
    using Util;

    using Newbe.Mahua;
    using Newbe.Mahua.MahuaEvents;

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
            RabbitMQService.Instance.Send(
                new MsgInformation
                    {
                        Id = Guid.NewGuid().ToString(),
                        Information = AiInformation.AuthCode,
                        Msg = Utility.GetAuthCode(),
                        Time = DateTime.Now,
                        AiNum = Utility.SelfQQNum
                    });
        }
    }
}
