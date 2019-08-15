using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public abstract class DbBaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
