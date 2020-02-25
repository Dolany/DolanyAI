using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    public class CmdRec : DbBaseEntity
    {
        public string Command { get; set; }

        public string BindAi { get; set; }

        public long GroupNum { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; } = DateTime.Now;

        public string FunctionalAi { get; set; }

        public void Insert()
        {
            MongoService<CmdRec>.Insert(this);
        }

        public static List<CmdRec> RecentCmds(int hour)
        {
            var startTime = DateTime.Now.AddHours(-hour);
            return MongoService<CmdRec>.Get(p => p.Time >= startTime);
        }

        public static long RecentCmdsCount(int hour)
        {
            var startTime = DateTime.Now.AddHours(-hour);
            return MongoService<CmdRec>.Count(p => p.Time >= startTime);
        }
    }
}
