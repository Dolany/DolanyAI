using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipArmer : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<ArmerModel> Armers { get; set; } = new List<ArmerModel>();

        public static VipArmer Get(long QQNum)
        {
            var rec = MongoService<VipArmer>.GetOnly(p => p.QQNum == QQNum);
            if (rec == null)
            {
                rec = new VipArmer(){QQNum = QQNum};
                MongoService<VipArmer>.Insert(rec);
            }

            rec.Armers.Remove(p => p.ExpiryTime != null && p.ExpiryTime < DateTime.Now);
            return rec;
        }

        public bool CheckArmer(string armerName)
        {
            return Armers.Any(a => a.Name == armerName);
        }

        public void Update()
        {
            MongoService<VipArmer>.Update(this);
        }
    }

    public class ArmerModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ExpiryTime { get; set; }
    }
}
