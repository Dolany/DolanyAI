using System;
using System.Collections.Generic;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SoldierGroup : DbBaseEntity
    {
        public long Owner { get; set; }

        public string Name { get; set; }

        public SoldierState State { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastFeedTime { get; set; } = DateTime.Now;

        public int Count { get; set; }

        public void Die(int count)
        {
            if (count >= Count)
            {
                MongoService<SoldierGroup>.Delete(this);
            }
            else
            {
                Count -= count;
                MongoService<SoldierGroup>.Update(this);
            }
        }

        public void Die()
        {
            Die(Count);
        }

        public void Starve()
        {
            State = SoldierState.Starving;
            MongoService<SoldierGroup>.Update(this);
        }

        public void Rebel()
        {
            CreateTime = DateTime.Now;
            State = SoldierState.Rebel;
            Owner = 0;
            Name = $"【叛军】第{GroupNamingRec.GetNextNo(0)}军";
            MongoService<SoldierGroup>.Update(this);
        }

        public static List<SoldierGroup> Get(long QQNum)
        {
            return MongoService<SoldierGroup>.Get(p => p.Owner == QQNum);
        }

        public static IEnumerable<SoldierGroup> OverDateRebels(int overDays)
        {
            var overTime = DateTime.Now.AddDays(-overDays);
            return MongoService<SoldierGroup>.Get(p => p.State == SoldierState.Rebel && p.CreateTime <= overTime);
        }

        public void Insert()
        {
            MongoService<SoldierGroup>.Insert(this);
        }
    }

    public enum SoldierState
    {
        Working = 0,
        Starving = 1,
        Rebel = 2
    }
}
