using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Cache
{
    public class GlobalVarRecord : DbBaseEntity
    {
        public string Key { get; set; }

        public string Value { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpiryTime { get; set; }

        public static GlobalVarRecord Get(string key)
        {
            var record = MongoService<GlobalVarRecord>.GetOnly(p => p.Key == key);
            if (record != null)
            {
                return record;
            }

            record = new GlobalVarRecord(){Key = key};
            MongoService<GlobalVarRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<GlobalVarRecord>.Update(this);
        }
    }
}
