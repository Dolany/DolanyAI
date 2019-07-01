using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Doremi.OnlineStore
{
    public class OSPerson : DbBaseEntity
    {
        public long QQNum { get; set; }

        public int Golds { get; set; }

        public Dictionary<string, int> GiftDic { get; set; }

        public static OSPerson GetPerson(long QQNum)
        {
            var osPerson = MongoService<OSPerson>.GetOnly(p => p.QQNum == QQNum);
            if (osPerson != null)
            {
                return osPerson;
            }

            osPerson = new OSPerson {QQNum = QQNum};
            MongoService<OSPerson>.Insert(osPerson);

            return osPerson;
        }

        public void GiftIncome(string GiftName, int count = 1)
        {
            if (GiftDic == null)
            {
                GiftDic = new Dictionary<string, int>();
            }

            if (GiftDic.ContainsKey(GiftName))
            {
                GiftDic[GiftName] += count;
            }

            GiftDic.Add(GiftName, count);
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

        public void Update()
        {
            GiftDic.Remove(g => g == 0);

            MongoService<OSPerson>.Update(this);
        }
    }
}
