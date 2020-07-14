using Dolany.Database;
using MongoDB.Driver;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class GroupNamingRec : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int SequeceNo { get; set; }

        public static int GetNextNo(long QQNum)
        {
            var update = Builders<GroupNamingRec>.Update.Inc(p => p.SequeceNo, 1);
            var rec = MongoService<GroupNamingRec>.FindOneAndUpdate(p => p.QQNum == QQNum, update, true);
            return rec.SequeceNo;
        }
    }
}
