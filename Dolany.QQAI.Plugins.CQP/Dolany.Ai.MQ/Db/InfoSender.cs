using Dolany.Ai.Util;

namespace Dolany.Ai.MQ.Db
{
    public class InfoSender
    {
        private static readonly object lock_obj = new object();

        public static void Send(InformationType Information, string Msg = "", string RelationId = "", long FromGroup = 0L, long FromQQ = 0L)
        {
            lock (lock_obj)
            {
                RabbitMQService.Instance.Send(
                    new MsgInformation
                    {
                        FromGroup = FromGroup,
                        FromQQ = FromQQ,
                        Msg = Msg,
                        RelationId = RelationId,
                        Information = Information
                    });
            }
        }
    }
}
