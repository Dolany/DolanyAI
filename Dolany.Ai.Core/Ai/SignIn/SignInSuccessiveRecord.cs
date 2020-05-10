using System;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.UtilityTool;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolany.WorldLine.Standard.Ai.Game.SignIn
{
    public class SignInSuccessiveRecord : DbBaseEntity
    {
        public long QQNum { get; set; }

        public long GroupNum { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EndDate { get; set; }

        public int SuccessiveDays => (int) (EndDate - StartDate).TotalDays + 1;

        public bool IsYesterdaySigned => EndDate.Date == DateTime.Today.AddDays(-1);

        private static SignInSuccessiveRecord GetLastest(long GroupNum, long QQNum)
        {
            return MongoService<SignInSuccessiveRecord>.Get(p => p.GroupNum == GroupNum && p.QQNum == QQNum).OrderByDescending(p => p.EndDate).FirstOrDefault();
        }

        public static bool IsTodaySigned(long GroupNum, long QQNum)
        {
            return GetLastest(GroupNum, QQNum)?.EndDate == DateTime.Today;
        }

        public static SignInSuccessiveRecord Sign(long GroupNum, long QQNum)
        {
            var lastest = GetLastest(GroupNum, QQNum);
            if (lastest == null || !lastest.IsYesterdaySigned)
            {
                var newSign = new SignInSuccessiveRecord {QQNum = QQNum, GroupNum = GroupNum, StartDate = DateTime.Today, EndDate = DateTime.Today};
                MongoService<SignInSuccessiveRecord>.Insert(newSign);
                return newSign;
            }

            lastest.EndDate = DateTime.Today;
            lastest.Update();
            return lastest;
        }

        public void Update()
        {
            MongoService<SignInSuccessiveRecord>.Update(this);
        }

        public void Delete()
        {
            MongoService<SignInSuccessiveRecord>.Delete(this);
        }

        public static SignInSuccessiveRecord MakeUp(long GroupNum, long QQNum)
        {
            var lastTwo = MongoService<SignInSuccessiveRecord>.Get(p => p.GroupNum == GroupNum && p.QQNum == QQNum).OrderByDescending(p => p.EndDate).Take(2).ToList();
            if (lastTwo.IsNullOrEmpty())
            {
                return null;
            }

            if (lastTwo.Count == 1 || lastTwo[0].StartDate > lastTwo[1].EndDate.AddDays(1))
            {
                lastTwo[0].StartDate = lastTwo[0].StartDate.AddDays(-1);
                lastTwo[0].Update();
                return lastTwo[0];
            }

            lastTwo[0].StartDate = lastTwo[1].StartDate;
            lastTwo[0].Update();
            lastTwo[1].Delete();

            return lastTwo[0];
        }
    }
}
