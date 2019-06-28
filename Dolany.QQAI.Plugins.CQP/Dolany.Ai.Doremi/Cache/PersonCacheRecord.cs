using System;
using Dolany.Database;

namespace Dolany.Ai.Doremi.Cache
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
            if (record != null)
            {
                return record;
            }

            record = new PersonCacheRecord(){QQNum = QQNum, Key = Key};
            MongoService<PersonCacheRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<PersonCacheRecord>.Update(this);
        }
    }
}
