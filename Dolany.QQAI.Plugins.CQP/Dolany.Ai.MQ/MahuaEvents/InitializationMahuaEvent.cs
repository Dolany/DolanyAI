namespace Dolany.Ai.MQ.MahuaEvents
{
    using Util;

    using Newbe.Mahua;
    using Newbe.Mahua.MahuaEvents;

    /// <summary>
    /// 插件初始化事件
    /// </summary>
    public class InitializationMahuaEvent
        : IInitializationMahuaEvent
    {
        public IMahuaApi MahuaApi { get; }

        public InitializationMahuaEvent(
            IMahuaApi mahuaApi)
        {
            MahuaApi = mahuaApi;
        }

        public void Initialized(InitializedContext context)
        {
            RabbitMQService.Instance.Send(
                new MsgInformation
                    {
                        Information = InformationType.AuthCode,
                        Msg = Utility.GetAuthCode()
                    });
        }
    }
}
