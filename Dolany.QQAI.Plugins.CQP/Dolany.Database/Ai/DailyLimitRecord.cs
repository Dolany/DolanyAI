using System;
using Dolany.Ai.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class DailyLimitRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Command { get; set; }

        public int Times { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExpiryTime { get; set; }

        public static DailyLimitRecord Get(long QQNum, string Command)
        {
            var record = MongoService<DailyLimitRecord>.GetOnly(p => p.QQNum == QQNum && p.Command == Command);
            if (record != null)
            {
                return record;
            }

            record = new DailyLimitRecord(){QQNum = QQNum, Command = Command, ExpiryTime = CommonUtil.UntilTommorow()};
            MongoService<DailyLimitRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<DailyLimitRecord>.Update(this);
        }

        public bool Check(int times)
        {
            return Times < times;
        }

        public void Cache()
        {
            Times++;
        }

        public void Decache(int count = 1)
        {
            Times -= count;
        }
    }
}
