namespace Dolany.Ai.MQ.Db
{
    using System;

    public class InfoSender
    {
        public static void Send(string Information, string Msg = "", string RelationId = "", long FromGroup = 0L, long FromQQ = 0L)
        {
            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(
                    new MsgInformation
                        {
                            Id = Guid.NewGuid().ToString(),
                            FromGroup = FromGroup,
                            FromQQ = FromQQ,
                            Msg = Msg,
                            RelationId = RelationId,
                            Time = DateTime.Now,
                            Information = Information,
                            AiNum = Utility.SelfQQNum
                        });

                db.SaveChanges();
            }
        }
    }
}
