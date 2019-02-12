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

        public static void GoldIncome(long QQNum, int gold)
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
        }

        public static void GoldConsume(long QQNum, int gold)
        {
            var person = GetPerson(QQNum);
            person.Golds -= gold;
            MongoService<OSPerson>.Update(person);
        }

        public bool CheckBuff(string buffName)
        {
            return Buffs != null && Buffs.Any(b => b.Name == buffName && b.ExpiryTime.ToLocalTime() > DateTime.Now);
        }

        public static void AddBuff(long qqNum, string name, string description, bool isPos, int expiryHours)
        {
            var osPerson = MongoService<OSPerson>.Get(p => p.QQNum == qqNum).FirstOrDefault();
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = qqNum, Buffs = new List<OSPersonBuff>(){new OSPersonBuff
                {
                    Name = name,
                    Description = description,
                    ExpiryTime = DateTime.Now.AddHours(expiryHours),
                    IsPositive = isPos
                }}};
                MongoService<OSPerson>.Insert(osPerson);

                return;
            }

            if (osPerson.Buffs == null)
            {
                osPerson.Buffs = new List<OSPersonBuff>();
            }
            var buff = osPerson.Buffs.FirstOrDefault(b => b.Name == name);
            if (buff == null)
            {
                osPerson.Buffs.Add(new OSPersonBuff
                {
                    Name = name,
                    Description = description,
                    ExpiryTime = DateTime.Now.AddHours(expiryHours),
                    IsPositive = isPos
                });
            }
            else
            {
                buff.ExpiryTime = DateTime.Now.AddHours(expiryHours);
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
