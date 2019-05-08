using System;
using Dolany.Database;

namespace Dolany.Ai.Core.Cache
{
    public class PersonCacheRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public static PersonCacheRecord Get(long QQNum, string Key)
        {
            var record = MongoService<PersonCacheRecord>.GetOnly(p => p.QQNum == QQNum && p.Key == Key);
            if (record == null)
            {
                record = new PersonCacheRecord(){QQNum = QQNum, Key = Key};
                MongoService<PersonCacheRecord>.Insert(record);
            }

            if (record.ExpiryTime.HasValue && record.ExpiryTime.Value < DateTime.Now)
            {
                record.Value = string.Empty;
            }

            return record;
        }

        public void Update()
        {
            MongoService<PersonCacheRecord>.Update(this);
        }
    }
}
