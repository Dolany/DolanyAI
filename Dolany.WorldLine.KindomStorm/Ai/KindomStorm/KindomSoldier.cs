using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SoldierGroup
    {
        private List<KindomSoldier> Soldiers { get; set; }

        public SoldierGroup(long QQNum)
        {
            Soldiers = MongoService<KindomSoldier>.Get(p => p.Owner == QQNum);
        }

        public SoldierGroup(IEnumerable<KindomSoldier> Soldiers)
        {
            this.Soldiers = Soldiers.ToList();
        }

        public void Dead()
        {
            MongoService<KindomSoldier>.DeleteMany(Soldiers);
        }

        public void Starve()
        {
            var update = Builders<KindomSoldier>.Update.Set(p => p.State, SoldierState.Starving);
            var ids = Soldiers.Select(s => s.Id).ToArray();
            MongoService<KindomSoldier>.UpdateMany(p => ids.Contains(p.Id), update);
        }

        public void Rebel()
        {
            var update = Builders<KindomSoldier>.Update.Set(p => p.State, SoldierState.Rebel).Set(p => p.Owner, 0);
            var ids = Soldiers.Select(s => s.Id).ToArray();
            MongoService<KindomSoldier>.UpdateMany(p => ids.Contains(p.Id), update);
        }
    }

    public class KindomSoldier : DbBaseEntity
    {
        public long Owner { get; set; }

        public SoldierState State { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastFeedTime { get; set; } = DateTime.Now;
    }

    public enum SoldierState
    {
        Working,
        Starving,
        Rebel
    }
}
