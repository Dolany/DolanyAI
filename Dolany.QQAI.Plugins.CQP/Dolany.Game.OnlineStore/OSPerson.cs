using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Game.OnlineStore
{
    public partial class OSPerson : BaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public IList<OSPersonBuff> Buffs { get; set; } = new List<OSPersonBuff>();

        public IList<OSPersonBuff> EffectiveBuffs => Buffs.IsNullOrEmpty() ? null : Buffs.Where(b => b.ExpiryTime.ToLocalTime() > DateTime.Now).ToList();

        public static OSPerson GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson>.GetOnly(p => p.QQNum == QQNum);
            if (osPerson == null)
            {
                osPerson = new OSPerson {QQNum = QQNum};
                MongoService<OSPerson>.Insert(osPerson);
            }

            if (osPerson.Level != 0)
            {
                return osPerson;
            }

            osPerson.Level = 1;
            osPerson.MaxHP = 50;

            return osPerson;
        }

        public static int GoldIncome(long QQNum, int gold)
        {
            var osPerson = GetPerson(QQNum);

            osPerson.Golds += gold;
            osPerson.Update();
            return osPerson.Golds;
        }

        public static int GoldConsume(long QQNum, int gold)
        {
            var person = GetPerson(QQNum);
            person.Golds -= gold;
            person.Update();

            return person.Golds;
        }

        public bool CheckBuff(string buffName)
        {
            return Buffs != null && Buffs.Any(b => b.Name == buffName && b.ExpiryTime.ToLocalTime() > DateTime.Now);
        }

        public void RemoveBuff(string buffName)
        {
            if (Buffs.IsNullOrEmpty())
            {
                return;
            }

            var buff = Buffs.FirstOrDefault(b => b.Name == buffName);
            if (buff == null)
            {
                return;
            }

            Buffs.Remove(buff);
        }

        public void AddBuff(OSPersonBuff osBuff)
        {
            if (Buffs == null)
            {
                Buffs = new List<OSPersonBuff>();
            }

            var buff = Buffs.FirstOrDefault(b => b.Name == osBuff.Name);
            if (buff == null)
            {
                Buffs.Add(osBuff);
            }
            else
            {
                buff.ExpiryTime = osBuff.ExpiryTime;
            }
        }

        public void Update()
        {
            Buffs.Remove(b => b.ExpiryTime.ToLocalTime() < DateTime.Now || b.Data == 0);

            MongoService<OSPerson>.Update(this);
        }
    }

    public class OSPersonBuff
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsPositive { get; set; }
        public int Data { get; set; } = 1;
        public long Source { get; set; }
    }
}
