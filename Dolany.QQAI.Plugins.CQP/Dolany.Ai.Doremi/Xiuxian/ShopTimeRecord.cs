using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Doremi.Xiuxian
{
    public class ShopTimeRecord
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OpenTime { get; set; }

        public string[] SellingGoods { get; set; }
    }
}
