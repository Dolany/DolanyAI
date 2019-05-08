using System;
using Dolany.Database;

namespace Dolany.Ai.Core.Cache
{
    public class GlobalVarRecord : DbBaseEntity
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public static GlobalVarRecord Get(string key)
        {
            var record = MongoService<GlobalVarRecord>.GetOnly(p => p.Key == key);
            if (record == null)
            {
                record = new GlobalVarRecord(){Key = key};
                MongoService<GlobalVarRecord>.Insert(record);
            }

            if (record.ExpiryTime.HasValue && record.ExpiryTime.Value < DateTime.Now)
            {
                record.Value = string.Empty;
            }

            return record;
        }

        public void Update()
        {
            MongoService<GlobalVarRecord>.Update(this);
        }
    }
}
