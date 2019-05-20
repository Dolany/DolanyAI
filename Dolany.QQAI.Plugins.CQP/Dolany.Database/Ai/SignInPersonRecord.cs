using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.Database.Ai
{
    [BsonIgnoreExtraElements]
    public class SignInPersonRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<string, SignInPersonGroupInfo> GroupInfos { get; set; } = new Dictionary<string, SignInPersonGroupInfo>();

        public static SignInPersonRecord Get(long QQNum)
        {
            var record = MongoService<SignInPersonRecord>.GetOnly(p => p.QQNum == QQNum);
            if (record != null)
            {
                return record;
            }

            record = new SignInPersonRecord(){QQNum = QQNum};
            MongoService<SignInPersonRecord>.Insert(record);

            return record;
        }

        public void Update()
        {
            MongoService<SignInPersonRecord>.Update(this);
        }
    }

    [BsonIgnoreExtraElements]
    public class SignInPersonGroupInfo
    {
        public int SuccessiveDays { get; set; }

        public DateTime? LastSignInDate { get; set; }
    }
}
