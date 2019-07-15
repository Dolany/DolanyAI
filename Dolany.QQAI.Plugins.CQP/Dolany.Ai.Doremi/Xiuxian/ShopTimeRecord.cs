using System;
using System.Collections.Generic;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ShopTimeRecord : DbBaseEntity
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OpenTime { get; set; }

        public string[] SellingGoods { get; set; }

        public static List<ShopTimeRecord> TodayRecords()
        {
            var today = DateTime.Now;
            var nextDay = today.AddDays(1);

            return MongoService<ShopTimeRecord>.Get(p => p.OpenTime >= today && p.OpenTime <= nextDay);
        }
    }
}
