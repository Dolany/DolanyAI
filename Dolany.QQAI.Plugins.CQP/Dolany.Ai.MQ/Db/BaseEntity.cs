namespace Dolany.Ai.MQ.Db
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    using Newtonsoft.Json;

    [Serializable]
    [BsonIgnoreExtraElements]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseEntity
    {
        public string Id { get; set; }
    }
}
