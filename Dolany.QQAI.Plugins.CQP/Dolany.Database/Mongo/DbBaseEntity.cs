﻿namespace Dolany.Database
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    using Newtonsoft.Json;

    [Serializable]
    [BsonIgnoreExtraElements]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class DbBaseEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}