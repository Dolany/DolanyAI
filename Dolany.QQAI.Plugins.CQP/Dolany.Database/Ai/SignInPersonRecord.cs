using System;
using System.Collections.Generic;

namespace Dolany.Database.Ai
{
    public class SignInPersonRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public Dictionary<long, SignInGroupInfo> GroupInfos { get; set; } = new Dictionary<long, SignInGroupInfo>();

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

    public class SignInGroupInfo
    {
        public int SuccessiveDays { get; set; }

        public DateTime? LastSignInDate { get; set; }
    }
}
