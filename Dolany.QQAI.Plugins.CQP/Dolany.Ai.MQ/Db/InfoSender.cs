namespace Dolany.Ai.MQ.Db
{
    using System;

    public class InfoSender
    {
        public static void Send(string Msg = "", string RelationId = "", long FromGroup = 0, long FromQQ = 0)
        {
            using (var db = new AIDatabaseEntities())
            {
                db.MsgInformation.Add(
                    new MsgInformation
                        {
                            Id = Guid.NewGuid().ToString(),
                            FromGroup = 0,
                            FromQQ = 0,
                            Msg = Msg,
                            RelationId = RelationId,
                            Time = DateTime.Now
                        });

                db.SaveChanges();
            }
        }
    }
}
