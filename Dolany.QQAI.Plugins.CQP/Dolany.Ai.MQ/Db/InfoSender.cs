namespace Dolany.Ai.MQ.Db
{
    using System;
    using System.Threading.Tasks;

    public class InfoSender
    {
        private static readonly object lock_obj = new object();

        public static async Task Send(string Information, string Msg = "", string RelationId = "", long FromGroup = 0L, long FromQQ = 0L)
        {
            await Task.Run(
                () =>
                    {
                        lock (lock_obj)
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
                    });
        }
    }
}
