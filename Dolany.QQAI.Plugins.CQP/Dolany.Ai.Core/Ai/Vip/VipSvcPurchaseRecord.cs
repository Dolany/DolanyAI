using System;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipSvcPurchaseRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public string SvcName { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PurchaseTime { get; set; }

        public int Diamonds { get; set; }

        public void Insert()
        {
            MongoService<VipSvcPurchaseRecord>.Insert(this);
        }
    }
}
