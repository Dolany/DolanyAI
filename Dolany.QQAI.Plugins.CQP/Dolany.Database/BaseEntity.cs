using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Dolany.Database
{
    [Serializable]
    [BsonIgnoreExtraElements]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseEntity
    {
        [BsonElement("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
