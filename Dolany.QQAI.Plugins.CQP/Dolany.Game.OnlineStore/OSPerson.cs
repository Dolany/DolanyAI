using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Database;

namespace Dolany.Game.OnlineStore
{
    public class OSPerson : BaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public IList<OSPersonBuff> Buffs { get; set; }

        public DateTime? LastSignDate { get; set; }

        public int SuccessiveSignDays { get; set; }

        public static OSPerson GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum};
                MongoService<OSPerson>.Insert(osPerson);
            }

            return osPerson;
        }

        public static int GoldIncome(long QQNum, int gold)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum, Golds = gold};
                MongoService<OSPerson>.Insert(osPerson);
            }
            else
            {
                osPerson.Golds += gold;
                MongoService<OSPerson>.Update(osPerson);
            }

            return osPerson.Golds;
        }

        public static int GoldConsume(long QQNum, int gold)
        {
            var person = GetPerson(QQNum);
            person.Golds -= gold;
            MongoService<OSPerson>.Update(person);

            return person.Golds;
        }

        public bool CheckBuff(string buffName)
        {
            return Buffs != null && Buffs.Any(b => b.Name == buffName && b.ExpiryTime.ToLocalTime() > DateTime.Now);
        }

        public static void AddBuff(long qqNum, OSPersonBuff osBuff)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == qqNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = qqNum, Buffs = new List<OSPersonBuff>(){osBuff}};
                MongoService<OSPerson>.Insert(osPerson);

                return;
            }

            if (osPerson.Buffs == null)
            {
                osPerson.Buffs = new List<OSPersonBuff>();
            }
            var buff = osPerson.Buffs.FirstOrDefault(b => b.Name == osBuff.Name);
            if (buff == null)
            {
                osPerson.Buffs.Add(osBuff);
            }
            else
            {
                buff.ExpiryTime = osBuff.ExpiryTime;
            }

            MongoService<OSPerson>.Update(osPerson);
        }
    }

    public class OSPersonBuff
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsPositive { get; set; }
    }
}
