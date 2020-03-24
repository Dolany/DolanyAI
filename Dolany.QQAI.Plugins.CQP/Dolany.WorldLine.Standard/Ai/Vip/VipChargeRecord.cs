using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Vip
{
    public class VipChargeRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ChargeTime { get; set; }

        public double ChargeAmount { get; set; }

        public int DiamondAmount { get; set; }

        public string Message { get; set; }

        public string OrderID { get; set; }

        public void Insert()
        {
            MongoService<VipChargeRecord>.Insert(this);
        }
    }
}
