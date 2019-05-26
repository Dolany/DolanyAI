using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Dolany.Database
{
    [Serializable]
    [BsonIgnoreExtraElements]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class DbBaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
