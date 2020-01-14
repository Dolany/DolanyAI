using System;
using Dolany.Database;

namespace Dolany.WorldLine.Standard.Ai.Game.Shopping
{
    public class SignInGroupInfo : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public string DateStr { get; set; }

        public int IndexNo { get; set; }

        public static int GetAndUpdate(long GroupNum)
        {
            var todayStr = DateTime.Now.ToString("yyyyMMdd");
            var rec = MongoService<SignInGroupInfo>.GetOnly(p => p.GroupNum == GroupNum && p.DateStr == todayStr);
            if (rec == null)
            {
                rec = new SignInGroupInfo(){GroupNum = GroupNum, DateStr = todayStr};
                MongoService<SignInGroupInfo>.Insert(rec);
            }

            rec.IndexNo++;
            MongoService<SignInGroupInfo>.Update(rec);

            return rec.IndexNo;
        }
    }
}
