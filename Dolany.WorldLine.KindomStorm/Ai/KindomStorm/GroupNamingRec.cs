using Dolany.Database;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class GroupNamingRec : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int SequeceNo { get; set; }

        public static int GetNextNo(long QQNum)
        {
            var rec = MongoService<GroupNamingRec>.GetOnly(p => p.QQNum == QQNum);
            if (rec == null)
            {
                rec = new GroupNamingRec()
                {
                    QQNum = QQNum
                };
                MongoService<GroupNamingRec>.Insert(rec);
            }

            rec.SequeceNo++;
            MongoService<GroupNamingRec>.Update(rec);
            return rec.SequeceNo;
        }
    }
}
