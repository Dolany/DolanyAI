﻿using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class GroupSettings : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public string Name { get; set; }

        public bool IsPowerOn { get; set; } = true;

        public bool ForcedShutDown { get; set; }

        public string BindAi { get; set; }

        public List<string> BindAis { get; set; } = new List<string>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpiryTime { get; set; }

        public GroupAuthInfoModel AuthInfo { get; set; }

        public int MembersCount { get; set; }

        public Dictionary<string, string> AdditionSettings { get; set; } = new Dictionary<string, string>();

        public string WorldLine { get; set; }

        public static GroupSettings Get(long GroupNum)
        {
            return MongoService<GroupSettings>.GetOnly(p => p.GroupNum == GroupNum);
        }

        public void Update()
        {
            MongoService<GroupSettings>.Update(this);
        }

        public void Insert()
        {
            MongoService<GroupSettings>.Insert(this);
        }
    }

    [BsonIgnoreExtraElements]
    public class GroupAuthInfoModel
    {
        public long Owner { get; set; }

        public List<long> Mgrs { get; set; } = new List<long>();
    }
}
