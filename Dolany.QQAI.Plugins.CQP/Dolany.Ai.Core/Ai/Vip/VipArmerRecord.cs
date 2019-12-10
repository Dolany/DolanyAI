using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Ai.Core.Ai.Vip
{
    public class VipArmerRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public List<ArmerModel> Armers { get; set; } = new List<ArmerModel>();

        public static VipArmerRecord Get(long QQNum)
        {
            var rec = MongoService<VipArmerRecord>.GetOnly(p => p.QQNum == QQNum);
            if (rec == null)
            {
                rec = new VipArmerRecord(){QQNum = QQNum};
                MongoService<VipArmerRecord>.Insert(rec);
            }

            rec.Armers.Remove(p => p.ExpiryTime != null && p.ExpiryTime < DateTime.Now);
            return rec;
        }

        public bool CheckArmer(string armerName, int count = 1)
        {
            return Armers.Count(a => a.Name == armerName) >= count;
        }

        public void Update()
        {
            MongoService<VipArmerRecord>.Update(this);
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
