using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.KindomStorm.Ai.KindomStorm
{
    public class SoldierGroup : DbBaseEntity
    {
        public long Owner { get; set; }

        public string Name { get; set; }

        public SoldierState State { get; set; }

        public SoldierGroupLevel GroupLevel { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastFeedTime { get; set; } = DateTime.Now;

        public int Count { get; set; }

        public void Dead(int count)
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

        public void Starve()
        {
            State = SoldierState.Starving;
            MongoService<SoldierGroup>.Update(this);
        }

        public void Rebel()
        {
            State = SoldierState.Rebel;
            Owner = 0;
            MongoService<SoldierGroup>.Update(this);
        }
    }

    public enum SoldierState
    {
        Working = 0,
        Starving = 1,
        Rebel = 2
    }
}
